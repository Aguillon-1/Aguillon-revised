using System;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace CMS_Revised.Connections
{
    public class DatabaseConn
    {
        // LocalDB connection string for Database1.mdf in the project directory
        private const string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\danie\source\repos\Bobsi01\CMS_Revised\Connections\Database1.mdf;Integrated Security=True;Connect Timeout=30";

        public static SqlConnection GetConnection()
        {
            // Only create the connection, do not open it here
            var connection = new SqlConnection(ConnectionString);
            return connection;
        }
    }
}