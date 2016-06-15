using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Dupper.Firebird.Helpers;
using Dupper.Firebird.Properties;

namespace Dupper.Firebird.FBSQLHelpers
{
    public class FbQueryCreator<T> : DbHelpers
    {
        private readonly string _tableName;

        #region constructors

        public FbQueryCreator()
        {
            _tableName = typeof(T).GetAttributeValue((TableAttribute t) => t.Name);
        }

        public FbQueryCreator(string connectionstring) : base(connectionstring)
        {
            _tableName = typeof(T).GetAttributeValue((TableAttribute t) => t.Name);
        }

        public FbQueryCreator(string server, string database, string user, string pass)
            : base(server, database, user, pass)
        {
            _tableName = typeof(T).GetAttributeValue((TableAttribute t) => t.Name);
        }

        #endregion

        #region delete query

        /// <summary>
        /// Make query for delete records from table
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>System.String.</returns>
        /// <example>
        ///<code lang="cs" >
        ///try
        ///{
        ///   FbQueryCreator helpers = new FbQueryCreator(server: "localhost", 
        ///					                           database: @"d:\base\TEST.FDB", 
        ///					                           user: "sysdba", 
        ///      				                         pass: "masterkey");
        ///   string sql = helpers.SqlDelete("IBDAC_LOADED");
        ///   Console.WriteLine(sql);
        ///}
        ///catch (Exception e)
        ///{
        ///   Console.WriteLine(e.Message);
        ///}
        /// </code>
        /// </example>
        public string SqlDelete(string tableName)
        {
            //get primary keys for table
            var keys = GetPrimaryKeys(tableName);
            //make where clause
            var whereClause = MakeWhereClause(keys);
            //
            return SqlDelete(tableName, whereClause);
        }

        public string SqlDelete(string tableName, string clause)
        {
            if (!clause.ToUpper().Contains("WHERE") && !string.IsNullOrEmpty(clause))
                clause = $"WHERE {clause}";
            return ($@"DELETE FROM ""{tableName}"" {clause}").Trim();
        }

        public string SqlDeleteByAttr()
        {
            //get primary keys for table
            var keys = GetPrimaryKeysByAttr();
            //make where clause
            var whereClause = MakeWhereClause(keys);
            //
            return SqlDeleteByAttr(whereClause);
        }

        public string SqlDeleteByAttr(string clause)
        {
            if (!clause.ToUpper().Contains("WHERE") && !string.IsNullOrEmpty(clause))
                clause = $"WHERE {clause}";
            return ($@"DELETE FROM ""{_tableName}"" {clause}").Trim();
        }
        #endregion

        #region update query

        public string SqlUpdate(string tableName)
        {
            //get primary keys 
            var keys = GetPrimaryKeys(tableName);
            string clause = MakeWhereClause(keys);
            return SqlUpdate(tableName, clause);
        }

        public string SqlUpdate(string tableName, string clause)
        {
            if (!clause.ToUpper().Contains("WHERE") && !string.IsNullOrEmpty(clause))
                clause = $"WHERE {clause}";
            //get primary keys 
            var keys = GetPrimaryKeys(tableName);
            var keystr = string.Join(",", keys.Select(field => $@"""{field}"""));
            //get fields name except keys field
            var fields = GetFieldsName(tableName).Except(keys).Select(field => $@"    ""{field}"" = @{field}").ToList();
            if (fields.Count == 0)
                throw new Exception($"Not found fields for update {tableName}");
            string sql = $@"UPDATE ""{tableName}"" SET" + "\n"; //add head
            sql += string.Join(",\n", fields);                  //add fields 
            sql += clause;                                      //add clause 
            if (keys.Count == 1)
                sql += $"\nreturning {keystr};";
            return sql;
        }


        public string SqlUpdateByAttr()
        {
            //get primary keys 
            var keys = GetPrimaryKeysByAttr();
            string clause = MakeWhereClause(keys);
            return SqlUpdateByAttr(clause);
        }

        public string SqlUpdateByAttr(string clause)
        {
            if (!clause.ToUpper().Contains("WHERE") && !string.IsNullOrEmpty(clause))
                clause = $"WHERE {clause}";
            //get primary keys 
            var keys = GetPrimaryKeysByAttr();
            var keystr = string.Join(",", keys.Select(field => $@"""{field}"""));
            //get fields name except keys field
            var fields = GetFieldsNameByAttr().Except(keys).Select(field => $@"    ""{field}"" = @{field}").ToList();
            if (fields.Count == 0)
                throw new Exception($"Not found fields for update {_tableName}");
            string sql = $@"UPDATE ""{_tableName}"" SET" + "\n"; //add head
            sql += string.Join(",\n", fields);                  //add fields 
            sql += clause;                                      //add clause 
            if (keys.Count == 1)
                sql += $"\nreturning {keystr};";
            return sql;
        }
        #endregion

        #region insert query

        public string SqlInsert(string tableName)
        {
            //get fields name except keys field
            var fieldsparams = GetFieldsName(tableName);
            var fields = string.Join(",", fieldsparams.Select(field => $@"""{field}"""));
            var values = string.Join(",", fieldsparams.Select(field => $@"@{field}"));
            string sql = $@"insert into ""{tableName}"" ({fields}) values ({values});";
            return sql;
        }

        public string SqlInsertByAttr()
        {
            //get fields name except keys field
            var fieldsparams = GetFieldsNameByAttr();
            var fields = string.Join(",", fieldsparams.Select(field => $@"""{field}"""));
            var values = string.Join(",", fieldsparams.Select(field => $@"@{field}"));
            string sql = $@"insert into ""{_tableName}"" ({fields}) values ({values});";
            return sql;
        }
        #endregion

        #region update or insert

        public string SqlUpdateOrInsert(string tableName)
        {
            //get primary keys 
            var keys = GetPrimaryKeys(tableName);
            //get fields name except keys field
            var fieldsparams = GetFieldsName(tableName);
            var fields = string.Join(",", fieldsparams.Select(field => $@"""{field}"""));
            var values = string.Join(",", fieldsparams.Select(field => $@"@{field}"));
            var keystr = string.Join(",", keys.Select(field => $@"""{field}"""));
            string sql = $@"update or insert into ""{tableName}"" ({fields})" + "\n";
            sql += $"values ({values})\n";
            sql += $"matching ({keystr})";
            if (keys.Count == 1)
                sql += $"\nreturning {keystr};";
            return sql;
        }

        public string SqlUpdateOrInsertByAttr()
        {
            //get primary keys 
            var keys = GetPrimaryKeysByAttr();
            //get fields name except keys field
            var fieldsparams = GetFieldsNameByAttr();
            var fields = string.Join(",", fieldsparams.Select(field => $@"""{field}"""));
            var values = string.Join(",", fieldsparams.Select(field => $@"@{field}"));
            var keystr = string.Join(",", keys.Select(field => $@"""{field}"""));
            string sql = $@"update or insert into ""{_tableName}"" ({fields})" + "\n";
            sql += $"values ({values})\n";
            sql += $"matching ({keystr})";
            if (keys.Count == 1)
                sql += $"\nreturning {keystr};";
            return sql;
        }
        #endregion

        #region common methods

        /// <summary>
        /// returns all table names from database except system tables
        /// </summary>
        /// <returns>System.Collections.Generic.IList&lt;System.String&gt;.</returns>
        public IList<string> GetTablesName()
        {
            return Query<string>(Resources.SqlTablesNames, null).ToList();
        }

        /// <summary>
        /// returns collection Table
        /// </summary>
        /// <param name="withFields">include fields in collection</param>
        /// <returns>System.Collections.Generic.IList&lt;ConsoleApplication1.Table&gt;.</returns>
        public IList<Table> GetTables(bool withFields = true)
        {
            string sqlTables = Resources.SqlTablesNames;
            string sqlFields = Resources.SqlFieldsForTable;
            var tables = Query<Table>(sqlTables, null).ToList();
            if (withFields)
            {
                foreach (var table in tables)
                {
                    table.FieldList = Query<Fields>(sqlFields, new { TABLE_NAME = table.Name.Trim() });
                }
            }
            return tables.ToList();
        }

        /// <summary>
        /// returns collection Table
        /// </summary>
        /// <param name="withFields">include fields in collection</param>
        /// <returns>System.Collections.Generic.IList&lt;ConsoleApplication1.Table&gt;.</returns>
        public IList<Table> GetAureliusTables(bool withFields = true)
        {
            string sqlTables = Resources.SqlTablesNames;
            string sqlFields = Resources.SqlFieldsForTableDelphi;
            var tables = Query<Table>(sqlTables, null).ToList();
            if (withFields)
            {
                foreach (var table in tables)
                {
                    table.FieldList = Query<Fields>(sqlFields, new { TABLE_NAME = table.Name.Trim() });
                }
            }
            return tables.ToList();
        }

        /// <summary>
        /// returning collection of stored procedures
        /// </summary>
        /// <returns>System.Collections.Generic.IList&lt;System.String&gt;.</returns>
        public IList<StoredProcedure> GetSpName()
        {
            string sql = Resources.SqlStoredProcedures;
            return Query<StoredProcedure>(sql, null).ToList();
        }

        /// <summary>
        /// makes and returns clause by primary keys
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns>System.String.</returns>
        private string MakeWhereClause(IEnumerable<string> keys)
        {
            string whereClause = string.Empty;
            var enumerable = keys as string[] ?? keys.ToArray();
            if (enumerable.Count() == 1)
                whereClause = $@"WHERE (""{enumerable.FirstOrDefault()}"" = @{enumerable.FirstOrDefault()})";
            else if (enumerable.Count() > 1)
            {
                whereClause = "WHERE " + enumerable.Aggregate((x, y) => $@"(""{x}""= @{x}) and (""{y}""= @{y})");
            }
            return "\n" + whereClause;
        }

        /// <summary>
        /// returns the table's primary keys 
        /// </summary>
        /// <param name="tableName">The tablename.</param>
        /// <exception cref="System.Exception">Not found PRIMARY KEY</exception>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IList<string> GetPrimaryKeys(string tableName)
        {
            var keyList = Query<string>(Resources.SqlKeyFieldsForTable, new { TABLE_NAME = tableName }).ToList();
            if (keyList.Count == 0)
                throw new Exception("Not found PRIMARY KEY");
            return keyList.Select(s => s.Trim()).ToList();
        }

        public IList<string> GetPrimaryKeysByAttr()
        {
            IList<string> result = new List<string>();
            //get class properties with attribut [Key]
            var props = typeof(T).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(KeyAttribute)));
            foreach (var prop in props)
            {
                //get attribute [Column(name:"value")] 
                var attribute =
                    (prop.GetCustomAttributes().FirstOrDefault(a => a.GetType() == typeof(ColumnAttribute)) as ColumnAttribute);
                //get value for attribute [Column(name:"value")] 
                if (attribute != null)
                    result.Add(attribute.Name);
            }

            return result;
        }

        public IList<string> GetFieldsNameByAttr()
        {
            //get class properties with attribut [Column]
            var props = GetPropertysName(typeof(ColumnAttribute));
            return props.Select(prop => prop.GetAttributValue((ColumnAttribute a) => a.Name)).Where(attr => attr != null).ToList();
        }

        IEnumerable<PropertyInfo> GetPropertysName(Type attributeType)
        {
            return typeof(T).GetProperties().Where(prop => Attribute.IsDefined(prop, attributeType));
        }

        /// <summary>
        /// returns all field names for table
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>System.Collections.Generic.IList&lt;System.String&gt;.</returns>
        public IList<string> GetFieldsName(string tableName)
        {
            var fieldList = Query<string>(Resources.SqlFieldsName, new { TABLE_NAME = tableName });
            return fieldList.Select(s => s.Trim()).ToList();
        }
        public T FindByProperties(string sql, object properties)
        {
            return Query<T>(sql, properties).FirstOrDefault();
        }
        #endregion
    }
}
