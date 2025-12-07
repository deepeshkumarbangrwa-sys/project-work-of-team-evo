using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApplication1.Models; // VITAL: Connects to Models

namespace WebApplication1.Controllers
{
    public class FeedbackController : Controller
    {
        // Show the comments page
        public IActionResult ViewComments(int frameId)
        {
            ViewBag.FrameID = frameId;
            // Mock data to show functionality
            List<string> comments = new List<string>
            {
                "Patient (10:00 AM): I felt pain on the left side.",
                "Clinician (10:15 AM): Please adjust your cushion."
            };
            return View(comments);
        }

        // Handle Patient Comment Submission (P-4)
        [HttpPost]
        public IActionResult SubmitComment(int frameId, string commentText)
        {
            FeedbackManager.SubmitPatientComment(frameId, 123, commentText);
            return RedirectToAction("ViewComments", new { frameId = frameId });
        }

        // Handle Clinician Reply Submission (C-4)
        [HttpPost]
        public IActionResult SubmitReply(int parentCommentId, int frameId, string replyText)
        {
            FeedbackManager.SubmitClinicianReply(parentCommentId, 456, replyText);
            return RedirectToAction("ViewComments", new { frameId = frameId });
        }
    }
}