using EmployeeTable.Helper;
using EmployeeTable.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Windows.Forms;

namespace EmployeeTable.Controllers
{
    public class AdminController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["EmploymentDbContext"].ConnectionString;



        [AllowAnonymous]
        [HttpGet]
        public ActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateAdmin(Admin admin)
        {
            if (ModelState.IsValid)
            {
                bool emailExists = false;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand checkCmd = new SqlCommand("getAdmin", conn);
                    checkCmd.CommandType = CommandType.StoredProcedure;
                    checkCmd.Parameters.AddWithValue("@email", admin.email);

                    conn.Open();
                    SqlDataReader reader = checkCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (reader["email"].ToString().Equals(admin.email, StringComparison.OrdinalIgnoreCase))
                        {
                            emailExists = true;
                            break;
                        }
                    }

                    reader.Close();

                    if (emailExists)
                    {
                        ViewBag.Error = "Email already exists. Please choose a different one.";
                        return View(admin);
                    }
                    var singleAdmin = new[]
                    {
                        new
                        {
                            email = admin.email,
                            username = admin.username,
                            password = SecurityHelper.HashPassword(admin.password)
                        }
                    };

                    string json = JsonConvert.SerializeObject(singleAdmin);

                    SqlCommand createCmd = new SqlCommand("createAdminFromJson", conn);
                    createCmd.CommandType = CommandType.StoredProcedure;
                    createCmd.Parameters.AddWithValue("@json", json);
                    createCmd.ExecuteNonQuery();
                }

                TempData["Message"] = "Admin created successfully.";
                return RedirectToAction("Login");
            }

            return View(admin);
        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.Error = "You must log in first.";
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("getAdmin", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string storedPassword = reader["password"].ToString();
                    string hashedInput = SecurityHelper.HashPassword(password);

                    if (storedPassword == hashedInput)
                    {
                        FormsAuthentication.SetAuthCookie(reader["email"].ToString(), true);
                        HttpCookie usernameCookie = new HttpCookie("username", reader["username"].ToString());
                        usernameCookie.Expires = DateTime.Now.AddHours(1);
                        usernameCookie.HttpOnly = true;
                        Response.Cookies.Add(usernameCookie);
                        return RedirectToAction("Index", "Employee");
                    }
                }
            }

            ViewBag.Error = ("Invalid login credentials");
            return View();
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string CurrentPassword, string NewPassword)
        {
            string email = User.Identity.Name;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("getAdmin", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string storedPassword = reader["password"].ToString();
                    reader.Close();

                    string hashedInput = SecurityHelper.HashPassword(CurrentPassword);
                    if (storedPassword != hashedInput)
                    {
                        ViewBag.Error = "Current password is incorrect.";
                        return View();
                    }

                    var passwordChange = new[]
                    {
                new {
                    email = email,
                    password = SecurityHelper.HashPassword(NewPassword)
                }
            };

                    string json = JsonConvert.SerializeObject(passwordChange);

                    using (SqlCommand changeCmd = new SqlCommand("changeCurrentPasswordFromJson", conn))
                    {
                        changeCmd.CommandType = CommandType.StoredProcedure;
                        changeCmd.Parameters.AddWithValue("@json", json);
                        changeCmd.ExecuteNonQuery();
                    }

                    TempData["Message"] = "Password changed successfully.";
                    return RedirectToAction("Index", "Employee");
                }

                reader.Close();
                ViewBag.Error = "User not found or server error occurred.";
                return View();
            }
        }
    }
}