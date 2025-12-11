using Microsoft.AspNetCore.Mvc; // Removed the '#' symbol
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult ViewReport(int? patientId, DateTime startDate, DateTime endDate)
        {
            if (HttpContext.Session.GetString("UserRole") == null) return RedirectToAction("Login", "Account");

            int id = HttpContext.Session.GetInt32("UserID") ?? 0;
            int targetId = (HttpContext.Session.GetString("UserRole") == "Patient") ? id : patientId ?? id;

            ReportObject report = ReportGenerator.GenerateReportData(targetId, startDate, endDate);

            if (report.TotalAlerts > 0)
            {
                report = ReportGenerator.ExportAndStore(report);
            }
            return View(report);
        }

        public IActionResult DownloadPdf(string filePath)
        {
            // FIXED: Correct syntax is File.Exists(path), not File(path).Exists
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                return Content("Error: Report file could not be found.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileName = System.IO.Path.GetFileName(filePath);

            return File(fileBytes, "text/plain", fileName);
        }
    }
}