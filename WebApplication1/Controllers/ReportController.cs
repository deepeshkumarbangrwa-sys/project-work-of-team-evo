using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using WebApplication1.Models; // VITAL: This connects the Controller to your Models

namespace WebApplication1.Controllers
{
    public class ReportController : Controller
    {
        // Action to generate and view the report (User Story C-5)
        public IActionResult ViewReport(int patientId, DateTime startDate, DateTime endDate)
        {
            // 1. Call Model to do the math
            ReportObject report = ReportGenerator.GenerateReportData(patientId, startDate, endDate);

            // 2. If alerts exist, auto-generate the PDF reference (User Story A-3)
            if (report.TotalAlerts > 0)
            {
                report = ReportGenerator.ExportAndStore(report);
            }

            // 3. Send data to the View
            return View(report);
        }
    }
}