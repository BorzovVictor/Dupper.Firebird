using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using FirebirdSql.Data.FirebirdClient;

namespace Dupper.Firebird.Helpers
{
    public class DbHelpers
    {
        public string ConnectionString;

        #region constructors

        public DbHelpers(string connectionstring)
        {
            ConnectionString = connectionstring;
        }

        public IDbConnection FiebirdConnection => new FbConnection(ConnectionString);

        public DbHelpers(string server, string database, string user, string pass)
        {
            ConnectionString = (new FbConnectionStringBuilder
            {
                DataSource = server,
                Database = database,
                UserID = user,
                Password = pass
            }).ToString();
        }

        public DbHelpers()
        {
            //set default connection string
           /* _connectionString = (new FbConnectionStringBuilder
            {
                DataSource = "192.168.1.2",
                Database = @"d:\base\TEST.FDB",
                UserID = "sysdba",
                Password = "masterkey"
            }).ToString();*/
            ConnectionString = ConfigHelper.ReadConnectionString();
        }

        #endregion

        /// <summary>
        /// Executes a query, returning the data typed as per T
        /// </summary>
        /// <returns>A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public IEnumerable<T> Query<T>( string sql, object param)
        {
            using (var db = new FbConnection(ConnectionString))
            {
                return db.Query<T>(sql, param);
                
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param)
        {
            using (var db = new FbConnection(ConnectionString))
            {
                return await db.QueryAsync<T>(sql, param);

            }
        }

        /// <summary>
        /// Execute parameterized SQL  
        /// </summary>
        /// <returns>Number of rows affected</returns>
        public int Execute(string sql, object param)
        {
            using (var db = new FbConnection(ConnectionString))
            {
               return db.Execute(sql, param);
            }
        }
    }
}
