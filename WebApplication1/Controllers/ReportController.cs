// --- FILE: Controllers/ReportController.cs ---
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
// Assuming you have a placeholder for the DAL/Database access methods
// using WebApplication1.DataAccess; 

public class ReportController : Controller
{
    // C-5: Action to generate and view a report
    public IActionResult ViewReport(int patientId, DateTime startDate, DateTime endDate)
    {
        // NOTE: Add authorization check here (Clinician/Admin only)

        // 1. Generate the data model (Calls the logic in ReportGenerator.cs)
        ReportObject report = ReportGenerator.GenerateReportData(patientId, startDate, endDate);

        // 2. Export to PDF and store metadata (A-3)
        if (GetTotalAlerts(report) > 0)
        {
            // This method calls the PDF generation library and inserts the file path into the REPORT table.
            report = ReportGenerator.ExportAndStore(report);
        }

        // 3. Pass the resulting model to the View
        return View(report);
    }

    // A-3: Action to download the saved PDF file
    public IActionResult DownloadPdf(int reportId)
    {
        // 1. Fetch the FilePath from the REPORT table using reportId (Assumes a DAL method exists)
        // string filePath = SqlQueries.GetFilePathForReport(reportId); 

        // --- Mock file path retrieval for compilation ---
        var reportObj = new ReportObject();
        var reportIdProp = typeof(ReportObject).GetProperty("ReportID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (reportIdProp != null)
        {
            reportIdProp.SetValue(reportObj, reportId);
        }
        string filePath = null;
        var filePathProp = typeof(ReportObject).GetProperty("FilePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (filePathProp != null)
        {
            filePath = (string)filePathProp.GetValue(ReportGenerator.ExportAndStore(reportObj));
        }

        if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
        {
            // Use this if the file doesn't exist or path is missing
            return NotFound("Report file not found. ID: " + reportId);
        }

        // 2. Serve the file to the user
        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);

        return File(fileBytes, "application/pdf", fileName);
    }

    // Helper method to access the private TotalAlerts property via reflection
    private static int GetTotalAlerts(ReportObject report)
    {
        var prop = typeof(ReportObject).GetProperty("TotalAlerts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return prop != null ? (int)prop.GetValue(report) : 0;
    }
}