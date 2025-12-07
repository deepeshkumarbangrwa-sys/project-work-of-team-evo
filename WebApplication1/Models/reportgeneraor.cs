using System;
// We assume data comes from the database here.

namespace WebApplication1.Models
{
    public static class ReportGenerator
    {
        // USER STORY C-5: Calculate metrics for the report
        public static ReportObject GenerateReportData(int patientId, DateTime start, DateTime end)
        {
            // 1. In a real app, we would query the database here.
            // 2. For this assignment, we mock the logic to prove the structure works.

            // Mock Calculation Logic:
            double maxPressure = 195.5; // Example high pressure found
            int totalAlerts = 3;        // Example alert count

            string summary = $"Report generated for Patient {patientId}. " +
                             $"Period: {start.ToShortDateString()} - {end.ToShortDateString()}. " +
                             $"Status: {totalAlerts} high-risk events detected.";

            return new ReportObject
            {
                PatientID = patientId,
                StartTime = start,
                EndTime = end,
                MaxPressure = maxPressure,
                TotalAlerts = totalAlerts,
                SummaryText = summary
            };
        }

        // USER STORY A-3: Export to PDF and store metadata
        public static ReportObject ExportAndStore(ReportObject report)
        {
            // 1. Generate unique filename (Simulates PDF creation)
            string fileName = $"Report_{report.PatientID}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            // 2. Store the path (Simulates saving to server disk)
            report.FilePath = $@"C:\GrapheneTrace\Reports\{fileName}";

            // 3. Simulate getting a new ID from the database
            report.ReportID = new Random().Next(1000, 9999);

            return report;
        }
    }
}