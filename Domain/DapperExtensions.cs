using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Dupper.Firebird
{
    public static class DapperExtensions
    {
        public static int Insert<T>(this IDbConnection cnn, string tableName, dynamic param)
        {
            return SqlMapper.Execute(cnn, DynamicQuery.GetInsertQuery(tableName, param), param);
        }

        public static async Task<int> InsertAsync<T>(this IDbConnection cnn, string tableName, dynamic param)
        {
            return await SqlMapper.ExecuteAsync(cnn, DynamicQuery.GetInsertQuery(tableName, param), param);
        }

        public static void Update(this IDbConnection cnn, string tableName, dynamic param)
        {
            SqlMapper.Execute(cnn, DynamicQuery.GetUpdateQuery(tableName, param), param);
        }

        public static async Task UpdateAsync(this IDbConnection cnn, string tableName, dynamic param)
        {
           await SqlMapper.ExecuteAsync(cnn, DynamicQuery.GetUpdateQuery(tableName, param), param);
        }
    }
}
