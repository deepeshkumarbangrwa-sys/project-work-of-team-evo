using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using WebApplication1.Models;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class CommentModel
    {
        public string AuthorName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
    }

    public class PatientViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int NotificationCount { get; set; }
    }

    public class FeedbackController : Controller
    {
        public IActionResult PatientHome()
        {
            if (HttpContext.Session.GetString("UserRole") != "Patient") return RedirectToAction("Login", "Account");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        public IActionResult ClinicianDashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Clinician") return RedirectToAction("Login", "Account");

            var patients = new List<PatientViewModel>();
            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT u.UserID, u.FullName, 
                    (SELECT COUNT(*) FROM Comments c WHERE c.PatientID = u.UserID AND c.Role = 'Patient' AND c.IsRead = 0) as Unread
                    FROM Users u WHERE u.Role = 'Patient'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        patients.Add(new PatientViewModel
                        {
                            Id = (int)reader["UserID"],
                            Name = reader["FullName"].ToString(),
                            NotificationCount = (int)reader["Unread"]
                        });
                    }
                }
            }
            return View(patients);
        }

        public IActionResult ViewComments(int? patientId)
        {
            if (HttpContext.Session.GetString("UserRole") == null) return RedirectToAction("Login", "Account");

            int myId = HttpContext.Session.GetInt32("UserID") ?? 0;
            string role = HttpContext.Session.GetString("UserRole");

            int targetId = (role == "Patient") ? myId : patientId ?? 0;
            if (targetId == 0 && role == "Clinician") return RedirectToAction("ClinicianDashboard");

            if (role == "Clinician")
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    new SqlCommand($"UPDATE Comments SET IsRead = 1 WHERE PatientID = {targetId} AND Role = 'Patient'", conn).ExecuteNonQuery();
                }
            }

            var comments = new List<CommentModel>();
            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT AuthorName, Role, Content, Timestamp, IsRead FROM Comments WHERE PatientID = @pid ORDER BY Timestamp ASC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pid", targetId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comments.Add(new CommentModel
                            {
                                AuthorName = reader["AuthorName"].ToString(),
                                Role = reader["Role"].ToString(),
                                Content = reader["Content"].ToString(),
                                Timestamp = (DateTime)reader["Timestamp"],
                                IsRead = (bool)reader["IsRead"]
                            });
                        }
                    }
                }
            }

            ViewBag.CurrentRole = role;
            ViewBag.TargetPatientID = targetId;
            return View(comments);
        }

        [HttpPost]
        public IActionResult SubmitMessage(int patientId, string content)
        {
            if (HttpContext.Session.GetString("UserRole") == null) return RedirectToAction("Login", "Account");

            int myId = HttpContext.Session.GetInt32("UserID") ?? 0;
            string role = HttpContext.Session.GetString("UserRole");
            string name = HttpContext.Session.GetString("UserName");

            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Comments (PatientID, AuthorID, AuthorName, Role, Content, IsRead) VALUES (@pid, @aid, @aname, @role, @content, 0)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pid", patientId);
                    cmd.Parameters.AddWithValue("@aid", myId);
                    cmd.Parameters.AddWithValue("@aname", name);
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.Parameters.AddWithValue("@content", content);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("ViewComments", new { patientId = patientId });
        }
    }
}