using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using WebApplication1.Models;
using WebApplication1.Data; 
using System.Linq;
using System.Threading.Tasks; 
using Microsoft.EntityFrameworkCore; 

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
        private readonly AppDbContext _context; 

        public FeedbackController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Feedback/PatientHome
        public IActionResult PatientHome()
        {
            if (HttpContext.Session.GetString("UserRole") != "Patient") return RedirectToAction("Login", "Account");
            ViewBag.UserName = HttpContext.Session.GetString("UserName"); 
            return View();
        }

        // GET: /Feedback/ClinicianDashboard
        public async Task<IActionResult> ClinicianDashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Clinician") return RedirectToAction("Login", "Account");

            var patients = await _context.Users
                .Where(u => u.Role == "Patient")
                .Select(u => new PatientViewModel
                {
                    Id = u.UserId,
                    Name = u.FullName,
                    // Calculated the unread count via the navigation property
                    NotificationCount = u.FeedbackEntries
                        .Count(f => !f.IsReviewed) // Count unreviewed feedback
                })
                .ToListAsync();
            
            return View(patients);
        }

        // GET: /Feedback/ViewComments
        public async Task<IActionResult> ViewComments(int? patientId)
        {
            if (HttpContext.Session.GetString("UserRole") == null) return RedirectToAction("Login", "Account");

            // myId is now correctly retrieved as an integer
            int myId = HttpContext.Session.GetInt32("UserId") ?? 0;
            string role = HttpContext.Session.GetString("UserRole") ?? string.Empty;

            int targetId = (role == "Patient") ? myId : patientId ?? 0;
            if (targetId == 0) return RedirectToAction("ClinicianDashboard"); // Safety check for missing target

            if (role == "Clinician")
            {
                // Mark all patient-submitted comments as reviewed (assuming the Clinician is viewing them)
                var unreviewedComments = await _context.UserFeedback
                    .Where(f => f.UserId == targetId && !f.IsReviewed)
                    .ToListAsync();
                
                foreach (var comment in unreviewedComments)
                {
                    comment.IsReviewed = true;
                }
                await _context.SaveChangesAsync();
            }

            // Fetch all feedback related to the target user
            var feedback = await _context.UserFeedback
                .Where(f => f.UserId == targetId)
                // Include the User to get the FullName for the AuthorName property
                .Include(f => f.User) 
                .OrderBy(f => f.Timestamp)
                .Select(f => new CommentModel
                {
                    AuthorName = f.User.FullName, 
                    Role = f.User.Role, 
                    Content = f.CommentText,
                    Timestamp = f.Timestamp,
                    IsRead = f.IsReviewed 
                })
                .ToListAsync();

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

            // This ID is the AUTHOR's ID (the sender) - which is currently not used in the UserFeedback model
            // int currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0; 
            string role = HttpContext.Session.GetString("UserRole") ?? "Unknown";

            // If patientId is invalid, the foreign key fails. 
            // We assume patientId is valid since it comes from the URL/form.
            
            var newFeedback = new UserFeedback
            {
                // UserId is the Patient/Recipient ID (the entity this feedback belongs to)
                UserId = patientId, 
                Timestamp = DateTime.UtcNow,
                CommentText = content,
                IsReviewed = (role != "Patient") // Clinician replies are instantly reviewed
            };
            
            _context.UserFeedback.Add(newFeedback);
            
            // This is the line that will now succeed, as long as patientId is a valid User ID.
            await _context.SaveChangesAsync(); 
            
            return RedirectToAction("ViewComments", new { patientId = patientId });
        }
    }
}