﻿using EmployeeTable.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace EmployeeTable.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly string _connStr = ConfigurationManager.ConnectionStrings["EmploymentDbContext"].ConnectionString;

        private SqlConnection GetConnection() => new SqlConnection(_connStr);


        public JsonResult ViewDepartment()
        {
            try
            {
                var departments = new List<Department>();

                using (var con = GetConnection())
                using (var cmd = new SqlCommand("ShowDepartments", con) { CommandType = CommandType.StoredProcedure })
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            departments.Add(new Department
                            {
                                Deptid = Convert.ToInt32(reader["Deptid"]),
                                Deptname = reader["Deptname"].ToString()
                            });
                        }
                    }
                }

                return Json(new { data = departments }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                Response.StatusCode = 500;
                return Json(new { success = false, message = "Error retrieving departments" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult ViewEmployees()
        {
            try
            {
                var employees = new List<Employee>();

                using (var con = GetConnection())
                using (var cmd = new SqlCommand("ShowEmployees", con) { CommandType = CommandType.StoredProcedure })
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
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
                }

                return Json(new { data = employees }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                Response.StatusCode = 500;
                return Json(new { success = false, message = "Error retrieving employees" });
            }
        }


        [HttpPost]
        public JsonResult AddEmployee(Employee emp)
        {
            ModelState.Remove("id");

            if (!ModelState.IsValid || emp.dob >= DateTime.Today)
            {
                if (emp.dob >= DateTime.Today)
                    ModelState.AddModelError("dob", "Date of birth cannot be in the future");

                var errors = ModelState
                    .Where(kv => kv.Value.Errors.Count > 0)
                    .Select(kv => new
                    {
                        Field = kv.Key,
                        Error = kv.Value.Errors.First().ErrorMessage
                    })
                    .ToList();

                return Json(new { success = false, errors = errors });
            }

            try
            {
                using (var con = GetConnection())
                using (var cmd = new SqlCommand("AddEmployeeFromJson", con) { CommandType = CommandType.StoredProcedure })
                {
                    con.Open();
                    AddEmployeeParameters(cmd, emp);
                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { success = false, message = "Error adding employee", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateEmployee(Employee emp)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return Json(new { success = false, errors });
            }

            try
            {
                var jsonInput = JsonConvert.SerializeObject(new[] { emp });

                using (var con = GetConnection())
                using (var cmd = new SqlCommand("UpdateEmployeeFromJson", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@json", jsonInput);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows == 0)
                        return Json(new { success = false, message = "Employee not found" });

                    return Json(new { success = true, message = "Employee updated successfully" });
                }
            }
            catch
            {
                Response.StatusCode = 500;
                return Json(new { success = false, message = "Error updating employee" });
            }
        }



        [HttpPost]
        public JsonResult DeleteEmployee(int id)
        {
            if (id <= 0)
                return Json(new { success = false, message = "Invalid employee ID" });

            try
            {
                var employeeToDelete = new[] { new { id = id } };
                string json = JsonConvert.SerializeObject(employeeToDelete);

                using (var con = GetConnection())
                using (var cmd = new SqlCommand("DeleteEmployeeByIdFromJson", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@json", json);
                    con.Open();

                    int rows = cmd.ExecuteNonQuery();

                    if (rows == 0)
                        return Json(new { success = false, message = "Employee not found" });

                    return Json(new { success = true, message = "Employee deleted successfully" });
                }
            }
            catch
            {
                Response.StatusCode = 500;
                return Json(new { success = false, message = "Error deleting employee" });
            }
        }



        [HttpGet]
        public JsonResult Edit(int id)
        {
            if (id <= 0)
                return Json(new { success = false, message = "Invalid ID" }, JsonRequestBehavior.AllowGet);

            try
            {
                Employee emp = null;

                var jsonInput = JsonConvert.SerializeObject(new[] { new { id = id } });

                using (var con = GetConnection())
                using (var cmd = new SqlCommand("selectEmployeeWithIdFromJson", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@json", jsonInput);
                    con.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            emp = new Employee
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
                                ZipCode = reader["ZipCode"].ToString()
                            };
                        }
                    }
                }

                if (emp == null)
                {
                    Response.StatusCode = 404;
                    return Json(new { success = false, message = "Employee not found" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, data = emp }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                Response.StatusCode = 500;
                return Json(new { success = false, message = "Error retrieving employee" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        public ActionResult Index() => View();

        private void AddEmployeeParameters(SqlCommand cmd, Employee emp)
        {
            var employee = new[]
            {
                new {
                    FirstName = emp.FirstName,
                    MiddleName = (object)emp.MiddleName ?? DBNull.Value,
                    LastName = emp.LastName,
                    DeptId = emp.DeptId,
                    dob = emp.dob,
                    Email = emp.dob,
                    Phone = emp.Phone,
                    StreetAddress = emp.StreetAddress,
                    City = emp.City,
                    State = emp.State,
                    Country = emp.Country,
                    ZipCode = emp.ZipCode
                }
            };

            string json = JsonConvert.SerializeObject(employee);
            cmd.Parameters.AddWithValue("@json", json);
     

        }
    }
}
