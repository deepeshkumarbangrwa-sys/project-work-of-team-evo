using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserRole") != null)
            {
                return (HttpContext.Session.GetString("UserRole") == "Clinician")
                    ? RedirectToAction("ClinicianDashboard", "Feedback")
                    : RedirectToAction("PatientHome", "Feedback");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT UserID, Role, FullName FROM Users WHERE Email = @e AND Password = @p";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@p", password);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            HttpContext.Session.SetInt32("UserID", (int)reader["UserID"]);
                            HttpContext.Session.SetString("UserRole", reader["Role"].ToString());
                            HttpContext.Session.SetString("UserName", reader["FullName"].ToString());

                            return (reader["Role"].ToString() == "Clinician")
                                ? RedirectToAction("ClinicianDashboard", "Feedback")
                                : RedirectToAction("PatientHome", "Feedback");
                        }
                    }
                }
            }
            ViewBag.Error = "Invalid Email or Password.";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string fullName, string email, string password, string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                ViewBag.Error = "Please select a registration type.";
                return View();
            }

            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();
                using (SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email = @e", conn))
                {
                    checkCmd.Parameters.AddWithValue("@e", email);
                    if ((int)checkCmd.ExecuteScalar() > 0)
                    {
                        ViewBag.Error = "This email is already registered.";
                        return View();
                    }
                }

                string insertQuery = "INSERT INTO Users (Email, Password, Role, FullName) VALUES (@e, @p, @r, @f)";
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@e", email);
                    insertCmd.Parameters.AddWithValue("@p", password);
                    insertCmd.Parameters.AddWithValue("@r", role);
                    insertCmd.Parameters.AddWithValue("@f", fullName);
                    insertCmd.ExecuteNonQuery();
                }
            }

            ViewBag.Success = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}