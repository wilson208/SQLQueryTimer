using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SQLQueryTimer.Model;

namespace SQLQueryTimer.Utilities
{
    public class QueryUtility
    {
        public static string GetQueryValue(string connectionString, string query, QueryType type)
        {
            try
            {
                if (type == QueryType.MicrosoftSqlServer)
                {

                    using (var conn = new SqlConnection(connectionString))
                    using (var command = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        return command.ExecuteScalar().ToString();
                    }
                }
                else if (type == QueryType.MySql)
                {
                    using (var conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        MySqlCommand command = new MySqlCommand(query, conn);
                        return command.ExecuteScalar().ToString();
                    }
                }
                else
                {
                    throw new NotSupportedException("Query Type Not Supported Yet");
                }
            }
            catch (SqlException e)
            {
                throw new QueryException(String.Format("Error executing query against database with connection string '{0}'.", connectionString), e);
            }
        }
    }

    public class QueryException : Exception
    {
        public QueryException(string message, Exception innerException) : base(message, innerException){}
    }
}
