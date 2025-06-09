using EmployeeTable.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace EmployeeTable.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewDepartment()
        {
            string connStr = ConfigurationManager.ConnectionStrings["EmploymentDbContext"].ConnectionString;
            List<Department> departments = new List<Department>();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("ShowDepartments", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    departments.Add(new Department
                    {
                        Deptid = Convert.ToInt32(rdr["DeptID"]),
                        Deptname = rdr["DeptName"].ToString()
                    });
                }
            }

            return View(departments);
        }
    }
}