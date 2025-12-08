using System;
using System.IO; // Required for file creation

namespace WebApplication1.Models
{
    public static class ReportGenerator
    {
        // C-5: Calculate metrics
        public static ReportObject GenerateReportData(int patientId, DateTime start, DateTime end)
        {
            // MOCK DATA (Simulating Database)
            double maxPressure = 195.5;
            int totalAlerts = 3;

            string summary = $"Report for Patient {patientId}. " +
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

        // A-3: Create real temporary PDF file
        public static ReportObject ExportAndStore(ReportObject report)
        {
            // 1. Generate unique filename
            string fileName = $"Report_{report.PatientID}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            // 2. Get a safe path on your computer (Temp folder)
            string tempPath = Path.GetTempPath();
            string filePath = Path.Combine(tempPath, fileName);

            // 3. ACTUALLY CREATE THE FILE so download works
            File.WriteAllText(filePath, $"Graphene Trace Report\n\n{report.SummaryText}");

            // 4. Update object
            report.FilePath = filePath;
            report.ReportID = new Random().Next(1000, 9999);

            return report;
        }
    }
}