using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dupper.Firebird.FBSQLHelpers;
using Dupper.Firebird.Helpers;

namespace Dupper.Firebird
{
    public abstract class Repository<T> : FbQueryCreator<T>, IRepository<T> where T : EntityBase
    {

        private string sqlDelete;
        private string sqlUpdate;
        private string sqlinsert;
        private string sqlUpdateOrInsert;
        private IList<string> keysList;
        public readonly string _tableName;



        internal IDbConnection Connection
        {
            get { return FiebirdConnection; }
        }

        public Repository()
        {
            _tableName = typeof(T).GetAttributeValue((TableAttribute t) => t.Name);
            //
            MakeSqls();
        } 

        private void MakeSqls()
        {
            sqlDelete = SqlDelete(_tableName);
            sqlinsert = SqlInsert(_tableName);
            sqlUpdate = SqlUpdate(_tableName);
            sqlUpdateOrInsert = SqlUpdateOrInsert(_tableName);
            keysList = GetPrimaryKeys(_tableName);
            
        }
        public Repository(string server, string database, string user, string pass) 
            :base(server, database, user, pass)
        {
            _tableName = typeof(T).GetAttributeValue((TableAttribute t) => t.Name);
            //
            MakeSqls();
        }

        internal virtual dynamic Mapping(T item)
        {
            return item;
        }

        public virtual T Add(T item)
        {
            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();
                var id = cn.Query<int>(sqlinsert, item).FirstOrDefault();
                return id == 0 ? null : item;
            }
        }

        public virtual int AddOrUpdate(T item)
        {
            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();
                return cn.Execute(sqlUpdateOrInsert, item);
            }
        }

        public virtual async Task<int> AddOrUpdateAsync(T item)
        {
            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();
                return await cn.ExecuteAsync(sqlUpdateOrInsert, item);
            }
        }

        public virtual async Task<bool> AddAsync(T item)
        {
            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();
                return await cn.ExecuteAsync(sqlinsert, item) > 0;
            }
        }

        public virtual object Update(T item)
        {
            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();
                return cn.ExecuteScalar<object>(sqlUpdate, item);
            }
        }

        public virtual async Task<object> UpdateAsync(T item)
        {
            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();
                return await cn.ExecuteScalarAsync<object>(sqlUpdate, item);
            }
        }

        public virtual void Remove(T item)
        {
            
            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();
                cn.Execute(sqlDelete, item);
            }
        }

        /// <summary>
        /// Removes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <example>Remove(new {ID = id})</example>
        public virtual void Remove(object id)
        {
            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();
                cn.Execute(sqlDelete, id);
            }
        }

        public virtual async Task RemoveAsync(T item)
        {
            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();
                await cn.ExecuteAsync(sqlDelete, item);
            }
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> items;
            // extract the dynamic sql query and parameters from predicate  c => c.Id == id
            QueryResult result = DynamicQuery.GetDynamicQuery(_tableName, predicate);

            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();
                items = cn.Query<T>(result.Sql, (object)result.Param);
            }

            return items;
        }

        public virtual IEnumerable<T> Find(string whereClause, object parameters)
        {
            var builder = new StringBuilder();
            builder.Append("SELECT * FROM ");
            builder.Append(_tableName);
            builder.Append(" WHERE ");
            builder.Append(whereClause);

            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();
                return cn.Query<T>(builder.ToString().TrimEnd(), parameters);
            }
        }

        public T FindById(int id)
        {
            T item = default(T);

            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();

                item = cn.Query<T>($"SELECT * FROM {_tableName} WHERE {nameof(id)}=@ID", new { ID = id }).SingleOrDefault();
            }

            return item;
        }

        public async Task<T> FindByIdAsync(int id)
        {
            IEnumerable<T> item = default(IEnumerable<T>);

            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();
                item = await cn.QueryAsync<T>($"SELECT * FROM {_tableName} WHERE {nameof(id)}=@ID", new { ID = id });
            }

            return item.FirstOrDefault();
        }

        public void Delete(object param)
        {
            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();
                cn.Execute(sqlDelete, param);
            }
        }

        public async Task DeleteAsync(object param)
        {
            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();
                await cn.ExecuteAsync(sqlDelete, param);
            }
        }

        public IEnumerable<T> GetAll()
        {
            IEnumerable<T> items;

            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();
                items = cn.Query<T>($"SELECT * FROM {_tableName}");
            }

            return items;
        }

        public IEnumerable<T> Query(string sql, object parameters)
        {
            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();
                return cn.Query<T>(sql, parameters);
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            IEnumerable<T> items = null;

            using (IDbConnection cn = Connection)
            {
                if (cn.State != ConnectionState.Open) cn.Open();
                items = await cn.QueryAsync<T>($"SELECT * FROM {_tableName}");
            }

            return items;
        }
    }
}
