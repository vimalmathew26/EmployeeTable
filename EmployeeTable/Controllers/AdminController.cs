using EmployeeTable.Helper;
using EmployeeTable.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
                    SqlCommand insertCmd = new SqlCommand("createAdmin", conn);
                    insertCmd.CommandType = CommandType.StoredProcedure;

                    insertCmd.Parameters.AddWithValue("@email", admin.email);
                    insertCmd.Parameters.AddWithValue("@username", admin.username);
                    insertCmd.Parameters.AddWithValue("@password", SecurityHelper.HashPassword(admin.password));

                    insertCmd.ExecuteNonQuery();
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
                        return RedirectToAction("Index","Employee");
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

                    if (storedPassword == hashedInput)
                    {
                        SqlCommand updateCmd = new SqlCommand("changeCurrentPassword", conn);
                        updateCmd.CommandType = CommandType.StoredProcedure;
                        updateCmd.Parameters.AddWithValue("@password", SecurityHelper.HashPassword(NewPassword));
                        updateCmd.Parameters.AddWithValue("@email", email);
                        updateCmd.ExecuteNonQuery();
                        TempData["Message"]  = "Password changed successfully.";
                        return RedirectToAction("Index", "Employee");
                    }
                    ViewBag.Error = "Current password is incorrect.";
                    return View();
                }
                ViewBag.Error = "Server error occured.";
                return View();

            }
        }
    }
}