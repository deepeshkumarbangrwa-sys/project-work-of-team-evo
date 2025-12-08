using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class CommentModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Role { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        // NEW: Tracks if the message has been seen
        public bool IsRead { get; set; } = false;
    }

    public class PatientViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NotificationCount { get; set; }
        public DateTime LastActivity { get; set; }
    }

    public class FeedbackController : Controller
    {
        // STATIC DB
        private static List<CommentModel> _comments = new List<CommentModel>
        {
            new CommentModel { Id=1, PatientId=123, AuthorId=123, AuthorName="John Doe", Role="Patient", Content="Pain in lower back.", Timestamp=DateTime.Now.AddHours(-5), IsRead=true },
            new CommentModel { Id=2, PatientId=123, AuthorId=456, AuthorName="Dr. Smith", Role="Clinician", Content="Adjust cushion tilt.", Timestamp=DateTime.Now.AddHours(-4), IsRead=true },
            
            // These messages are FALSE (Unread) by default. They will turn TRUE when you view them.
            new CommentModel { Id=3, PatientId=789, AuthorId=789, AuthorName="Alice Wonderland", Role="Patient", Content="I am feeling much better today!", Timestamp=DateTime.Now.AddMinutes(-30), IsRead=false },
            new CommentModel { Id=4, PatientId=789, AuthorId=789, AuthorName="Alice Wonderland", Role="Patient", Content="Can I increase the duration?", Timestamp=DateTime.Now.AddMinutes(-10), IsRead=false }
        };

        // --- CLINICIAN DASHBOARD ---
        public IActionResult ClinicianDashboard(string search)
        {
            string role = HttpContext.Session.GetString("UserRole");
            if (role != "Clinician") return RedirectToAction("ViewComments");

            var patientIds = _comments.Select(c => c.PatientId).Distinct().ToList();
            var dashboardList = new List<PatientViewModel>();

            foreach (var pid in patientIds)
            {
                var pComments = _comments.Where(c => c.PatientId == pid).ToList();

                // LOGIC: Count only messages that are from a Patient AND are NOT read yet
                int notifs = pComments.Count(c => c.Role == "Patient" && c.IsRead == false);

                dashboardList.Add(new PatientViewModel
                {
                    Id = pid,
                    Name = pComments.First(c => c.Role == "Patient").AuthorName,
                    NotificationCount = notifs,
                    LastActivity = pComments.Max(c => c.Timestamp)
                });
            }

            if (!string.IsNullOrEmpty(search))
            {
                dashboardList = dashboardList
                    .Where(p => p.Name.ToLower().Contains(search.ToLower()) || p.Id.ToString().Contains(search))
                    .ToList();
            }

            return View(dashboardList);
        }

        // --- CHAT VIEW ---
        public IActionResult ViewComments(int? patientId)
        {
            string role = HttpContext.Session.GetString("UserRole") ?? "Patient";
            int myId = HttpContext.Session.GetInt32("UserID") ?? 123;
            int targetPatientId;

            if (role == "Patient") targetPatientId = myId;
            else
            {
                if (patientId == null) return RedirectToAction("ClinicianDashboard");
                targetPatientId = patientId.Value;

                // --- MARK AS READ LOGIC ---
                // If a Clinician opens this page, find all unread messages for this patient and mark them Read.
                var unreadMsgs = _comments.Where(c => c.PatientId == targetPatientId && c.Role == "Patient" && !c.IsRead).ToList();
                foreach (var msg in unreadMsgs)
                {
                    msg.IsRead = true;
                }
                // --------------------------
            }

            var threadMessages = _comments
                .Where(c => c.PatientId == targetPatientId)
                .OrderBy(c => c.Timestamp)
                .ToList();

            ViewBag.TargetPatientID = targetPatientId;
            ViewBag.CurrentRole = role;

            return View(threadMessages);
        }

        // --- SUBMIT ACTIONS ---
        [HttpPost]
        public IActionResult SubmitComment(int patientId, string commentText)
        {
            int myId = HttpContext.Session.GetInt32("UserID") ?? 123;
            _comments.Add(new CommentModel
            {
                Id = _comments.Count + 1,
                PatientId = myId,
                AuthorId = myId,
                AuthorName = "Patient " + myId,
                Role = "Patient",
                Content = commentText,
                Timestamp = DateTime.Now,
                IsRead = false
            });
            return RedirectToAction("ViewComments");
        }

        [HttpPost]
        public IActionResult SubmitReply(int patientId, string replyText)
        {
            int myId = HttpContext.Session.GetInt32("UserID") ?? 456;
            _comments.Add(new CommentModel
            {
                Id = _comments.Count + 1,
                PatientId = patientId,
                AuthorId = myId,
                AuthorName = "Dr. Smith",
                Role = "Clinician",
                Content = replyText,
                Timestamp = DateTime.Now,
                IsRead = true
            });
            return RedirectToAction("ViewComments", new { patientId = patientId });
        }
    }
}