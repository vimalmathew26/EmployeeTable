using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using EmployeeTable.Models;

namespace EmployeeTable.Controllers
{
    public class EmployeeController : Controller
    {
        public JsonResult ViewDepartment()
        {
            List<Department> departments = new List<Department>();

            string connStr = ConfigurationManager.ConnectionStrings["EmploymentDbContext"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("ShowDepartments", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    departments.Add(new Department
                    {
                        Deptid = Convert.ToInt32(reader["Deptid"]),
                        Deptname = reader["Deptname"].ToString()
                    });
                }
            }

            return Json(new { data = departments }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ViewEmployees()
        {
            List<Employee> employees = new List<Employee>();
            string connStr = ConfigurationManager.ConnectionStrings["EmploymentDbContext"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("ShowEmployees", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    employees.Add(new Employee
                    {
                        id = Convert.ToInt32(reader["id"]),
                        FirstName = reader["FirstName"].ToString(),
                        MiddleName = reader["MiddleName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Deptname = reader["Deptname"].ToString()
                    });
                }
            }

            return Json(new { data = employees }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddEmployee(Employee emp)
        { 
               string connStr = ConfigurationManager.ConnectionStrings["EmploymentDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand cmd = new SqlCommand("AddEmployee", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FirstName", emp.FirstName);
                    cmd.Parameters.AddWithValue("@MiddleName", emp.MiddleName);
                    cmd.Parameters.AddWithValue("@LastName", emp.LastName);
                    cmd.Parameters.AddWithValue("@deptId", emp.DeptId);
                    cmd.Parameters.AddWithValue("@dob", emp.dob);
                    cmd.Parameters.AddWithValue("@Email", emp.Email);
                    cmd.Parameters.AddWithValue("@Phone", emp.Phone);
                    cmd.Parameters.AddWithValue("@StreetAddress", emp.StreetAddress);
                    cmd.Parameters.AddWithValue("@City", emp.City);
                    cmd.Parameters.AddWithValue("@State", emp.State);
                    cmd.Parameters.AddWithValue("@Country", emp.Country);
                    cmd.Parameters.AddWithValue("@Zipcode", emp.ZipCode);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                ViewBag.Message = "Employee added successfully!";
            return Json(new { success = true });
            }

        [HttpPost]
        public ActionResult DeleteEmployee(int id)
        {
            string connStr = ConfigurationManager.ConnectionStrings["EmploymentDbContext"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("DeleteEmployeeById", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                cmd.ExecuteNonQuery();

                return Json(new { success = true });


            }

        }

        [HttpGet]
        public JsonResult Edit(int id)
        {
            Employee employee = null;
            string connStr = ConfigurationManager.ConnectionStrings["EmploymentDbContext"].ConnectionString;


            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("selectEmployeeWithId", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {


                    while (reader.Read())
                    {
                        employee = new Employee
                        {
                            id = Convert.ToInt32(reader["id"]),
                            FirstName = reader["FirstName"].ToString(),
                            MiddleName = reader["MiddleName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            DeptId = Convert.ToInt32(reader["DeptId"]),
                            dob = Convert.ToDateTime(reader["dob"]),
                            Email = reader["Email"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            StreetAddress = reader["StreetAddress"].ToString(),
                            City = reader["City"].ToString(),
                            State = reader["State"].ToString(),
                            Country = reader["Country"].ToString(),
                            ZipCode = reader["Zipcode"].ToString()
                        };
                    }
                }

            }
            return Json(new {data=employee}, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult UpdateEmployee(Employee emp)
        {
            string connStr = ConfigurationManager.ConnectionStrings["EmploymentDbContext"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("UpdateEmployee", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", emp.id);
                cmd.Parameters.AddWithValue("@FirstName", emp.FirstName);
                cmd.Parameters.AddWithValue("@MiddleName", emp.MiddleName);
                cmd.Parameters.AddWithValue("@LastName", emp.LastName);
                cmd.Parameters.AddWithValue("@deptId", emp.DeptId);
                cmd.Parameters.AddWithValue("@dob", emp.dob);
                cmd.Parameters.AddWithValue("@Email", emp.Email);
                cmd.Parameters.AddWithValue("@Phone", emp.Phone);
                cmd.Parameters.AddWithValue("@StreetAddress", emp.StreetAddress);
                cmd.Parameters.AddWithValue("@City", emp.City);
                cmd.Parameters.AddWithValue("@State", emp.State);
                cmd.Parameters.AddWithValue("@Country", emp.Country);
                cmd.Parameters.AddWithValue("@ZipCode", emp.ZipCode);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return Json(new { success = true });
        }


        public ActionResult Index()
        {
            return View();
        }
    }
}
