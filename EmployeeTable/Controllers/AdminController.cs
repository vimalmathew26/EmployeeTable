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

        // GET: Admin
        [AllowAnonymous]
        public ActionResult GetAdmin() 
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult CreateAdmin()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult CreateAdmin(Admin admin)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("createAdmin", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@email", admin.email);
                    cmd.Parameters.AddWithValue("@username", admin.username);
                    cmd.Parameters.AddWithValue("@password", SecurityHelper.HashPassword(admin.password));

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                ViewBag.Message = "Admin created successfully.";
                return RedirectToAction("Login");
            }
            return View(admin);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
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
                        FormsAuthentication.SetAuthCookie(reader["username"].ToString(), false);
                        return RedirectToAction("Index","Employee");
                    }
                }
            }

            ViewBag.Error = "Invalid login credentials";
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
    }
}