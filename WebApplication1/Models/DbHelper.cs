using Microsoft.Data.SqlClient;

namespace WebApplication1.Models
{
    public static class DbHelper
    {
        // VERIFY THIS STRING MATCHES YOUR DATABASE IN SQL OBJECT EXPLORER
        private static string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GrapheneTraceDB;Integrated Security=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}