using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using WebApplication1.Models;
using WebApplication1.Data; // <-- NEW: Required for AppDbContext
using System.Linq;
using System.Threading.Tasks; // <-- NEW: For async database operations
using Microsoft.EntityFrameworkCore; // <-- NEW: For EF Core methods

namespace WebApplication1.Controllers
{
    // These models map directly to the UserFeedback table for display
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
        private readonly AppDbContext _context; // <-- NEW: Injected DbContext

        // Constructor for Dependency Injection
        public FeedbackController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Feedback/PatientHome
        public IActionResult PatientHome()
        {
            if (HttpContext.Session.GetString("UserRole") != "Patient") return RedirectToAction("Login", "Account");
            // NOTE: Ensure "UserName" is set in AccountController during login.
            ViewBag.UserName = HttpContext.Session.GetString("UserEmail"); 
            return View();
        }

        // GET: /Feedback/ClinicianDashboard
        public async Task<IActionResult> ClinicianDashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Clinician") return RedirectToAction("Login", "Account");

            // --- REPLACED MANUAL SQL WITH EF CORE LINQ ---
            var patients = await _context.Users
                .Where(u => u.Role == "Patient")
                .Select(u => new PatientViewModel
                {
                    Id = u.UserId,
                    Name = u.FullName,
                    // Calculated the unread count via the navigation property
                    NotificationCount = u.FeedbackEntries
                        .Count(f => f.User.Role == "Patient" && !f.IsReviewed) 
                })
                .ToListAsync();
            // ---------------------------------------------
            
            return View(patients);
        }

        // GET: /Feedback/ViewComments
        public async Task<IActionResult> ViewComments(int? patientId)
        {
            if (HttpContext.Session.GetString("UserRole") == null) return RedirectToAction("Login", "Account");

            int myId = HttpContext.Session.GetInt32("UserId") ?? 0;
            string role = HttpContext.Session.GetString("UserRole") ?? string.Empty;

            int targetId = (role == "Patient") ? myId : patientId ?? 0;
            if (targetId == 0 && role == "Clinician") return RedirectToAction("ClinicianDashboard");

            if (role == "Clinician")
            {
                // --- REPLACED MANUAL SQL UPDATE WITH EF CORE ---
                // Mark all patient-submitted (unreplied) comments as reviewed
                var unreviewedComments = await _context.UserFeedback
                    .Where(f => f.UserId == targetId && !f.IsReviewed)
                    .ToListAsync();
                
                foreach (var comment in unreviewedComments)
                {
                    comment.IsReviewed = true;
                }
                await _context.SaveChangesAsync();
                // ------------------------------------------------
            }

            // --- REPLACED MANUAL SQL QUERY WITH EF CORE LINQ ---
            // Fetch all feedback related to the target user
            var feedback = await _context.UserFeedback
                .Where(f => f.UserId == targetId)
                // Use the User navigation property for the AuthorName
                .OrderBy(f => f.Timestamp)
                .Select(f => new CommentModel
                {
                    AuthorName = f.User.FullName, 
                    Role = f.User.Role, 
                    Content = f.CommentText,
                    Timestamp = f.Timestamp,
                    IsRead = f.IsReviewed // Map IsReviewed to IsRead for display logic
                })
                .ToListAsync();
            // ---------------------------------------------------

            ViewBag.CurrentRole = role;
            ViewBag.TargetPatientID = targetId;
            ViewBag.PatientName = await _context.Users.Where(u => u.UserId == targetId).Select(u => u.FullName).FirstOrDefaultAsync();

            return View(feedback);
        }

        // POST: /Feedback/SubmitMessage
        [HttpPost]
        public async Task<IActionResult> SubmitMessage(int patientId, string content)
        {
            if (HttpContext.Session.GetString("UserRole") == null) return RedirectToAction("Login", "Account");
            if (string.IsNullOrWhiteSpace(content)) return RedirectToAction("ViewComments", new { patientId = patientId });

            int myId = HttpContext.Session.GetInt32("UserId") ?? 0;
            string role = HttpContext.Session.GetString("UserRole") ?? "Unknown";

            // --- REPLACED MANUAL SQL INSERT WITH EF CORE ---
            var newFeedback = new UserFeedback
            {
                UserId = patientId, 
                Timestamp = DateTime.UtcNow,
                CommentText = content,
                IsReviewed = (role != "Patient") // Clinician replies are instantly reviewed
            };
            
            _context.UserFeedback.Add(newFeedback);
            await _context.SaveChangesAsync();
            // ---------------------------------------------
            
            return RedirectToAction("ViewComments", new { patientId = patientId });
        }
    }
}