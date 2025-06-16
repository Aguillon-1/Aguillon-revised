using System;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace CMS_Revised.Connections
{
    public class DatabaseConn
    {
        // LocalDB connection string for Database1.mdf in the project directory
        //private const string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\danie\source\repos\Bobsi01\CMS_Revised\Connections\Database1.mdf;Integrated Security=True;Connect Timeout=30";
        private const string ConnectionString = @"Server=tcp:cmsrevised2526.database.windows.net,1433;Initial Catalog=CMS-revised-DB;Persist Security Info=False;User ID=cmsrevised2526;Password=CMSrevised25;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static SqlConnection GetConnection()
        {
            // Only create the connection, do not open it here
            var connection = new SqlConnection(ConnectionString);
            return connection;
        }
    }
}
