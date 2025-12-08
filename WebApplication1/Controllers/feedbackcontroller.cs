using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Controllers
{
    // A simple container for our chat data
    public class CommentModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; } // "Patient" or "Clinician"
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public int? ReplyToId { get; set; } // Links a reply to a specific comment
    }

    public class FeedbackController : Controller
    {
        // STATIC DATA: Acts as our temporary database
        private static List<CommentModel> _comments = new List<CommentModel>
        {
            new CommentModel { Id=1, UserId=123, UserName="John Doe", Role="Patient", Content="I felt a sharp pressure on my lower back around 2 PM.", Timestamp=DateTime.Now.AddHours(-2) },
            new CommentModel { Id=2, UserId=456, UserName="Dr. Smith", Role="Clinician", Content="Thank you, John. I see the alert. Please try adjusting your cushion tilt.", Timestamp=DateTime.Now.AddHours(-1), ReplyToId=1 }
        };

        public IActionResult ViewComments(int frameId)
        {
            ViewBag.FrameID = frameId;

            // 1. DETERMINE ROLE (Testing Mode)
            // We check the Session to see who you are pretending to be.
            string currentRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(currentRole))
            {
                currentRole = "Patient"; // Default to Patient if not set
                HttpContext.Session.SetString("UserRole", "Patient");
                HttpContext.Session.SetInt32("UserID", 123);
            }

            ViewBag.CurrentRole = currentRole;
            ViewBag.CurrentUserID = HttpContext.Session.GetInt32("UserID");

            return View(_comments);
        }

        // Helper Action: Lets you switch roles instantly to test both views
        public IActionResult SwitchRole(string role)
        {
            HttpContext.Session.SetString("UserRole", role);
            if (role == "Patient") HttpContext.Session.SetInt32("UserID", 123);
            else HttpContext.Session.SetInt32("UserID", 456);

            return RedirectToAction("ViewComments", new { frameId = 501 });
        }

        [HttpPost]
        public IActionResult SubmitComment(int frameId, string commentText)
        {
            int userId = HttpContext.Session.GetInt32("UserID") ?? 123;

            if (!string.IsNullOrEmpty(commentText))
            {
                _comments.Add(new CommentModel
                {
                    Id = _comments.Count + 1,
                    UserId = userId,
                    UserName = "Patient " + userId,
                    Role = "Patient",
                    Content = commentText,
                    Timestamp = DateTime.Now
                });
            }
            return RedirectToAction("ViewComments", new { frameId = frameId });
        }

        [HttpPost]
        public IActionResult SubmitReply(int parentCommentId, int frameId, string replyText)
        {
            int userId = HttpContext.Session.GetInt32("UserID") ?? 456;

            if (!string.IsNullOrEmpty(replyText))
            {
                _comments.Add(new CommentModel
                {
                    Id = _comments.Count + 1,
                    UserId = userId,
                    UserName = "Clinician " + userId,
                    Role = "Clinician",
                    Content = replyText,
                    Timestamp = DateTime.Now,
                    ReplyToId = parentCommentId
                });
            }
            return RedirectToAction("ViewComments", new { frameId = frameId });
        }
    }
}