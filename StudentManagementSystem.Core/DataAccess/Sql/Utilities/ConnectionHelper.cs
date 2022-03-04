using System;
using MySql.Data.MySqlClient;

namespace StudentManagementSystem.Core.DataAccess.Sql.Utilities
{
    public static class ConnectionHelper
    {
        public static MySqlConnection OpenConnection()
        {
            MySqlConnection connection = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["StudentManagementContext"].ConnectionString);
            try
            {
                connection.Open();
                return connection;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void CloseConnection(MySqlConnection connection)
        {
            connection.Close();
        }
    }
}
