using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ReportController : Controller
    {
        // C-5: View Report Page
        public IActionResult ViewReport(int? patientId, DateTime startDate, DateTime endDate)
        {
            // TESTING MODE: Default to User 123 if not logged in
            if (patientId == null || patientId == 0)
            {
                int? sessionID = HttpContext.Session.GetInt32("UserID");
                patientId = sessionID ?? 123; // Use 123 if session is null
            }

            // Generate Data
            ReportObject report = ReportGenerator.GenerateReportData(patientId.Value, startDate, endDate);

            // Generate PDF if alerts exist
            if (report.TotalAlerts > 0)
            {
                report = ReportGenerator.ExportAndStore(report);
            }

            return View(report);
        }

        // A-3: Download Action
        public IActionResult DownloadPdf(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                return Content("Error: File not found on server.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileName = System.IO.Path.GetFileName(filePath);

            return File(fileBytes, "text/plain", fileName);
        }
    }
}