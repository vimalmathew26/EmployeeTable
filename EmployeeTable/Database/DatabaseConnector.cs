using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;


namespace EmployeeTable.Database
{
    public class DatabaseConnector
    {
        public void Database()
        {
            String connStr = ConfigurationManager.ConnectionStrings["EmploymentDbContext"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr)) 
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Connected.");
                }
                catch 
                {
                    Console.WriteLine("Not connected.");
                }

            }


        }
    }
}