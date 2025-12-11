using System;
using System.IO;

namespace WebApplication1.Models
{
    public static class ReportGenerator
    {
        public static ReportObject GenerateReportData(int patientId, DateTime start, DateTime end)
        {
            return new ReportObject
            {
                PatientID = patientId,
                MaxPressure = 195.5,
                TotalAlerts = 3,
                SummaryText = $"Report for Patient {patientId}. Data is Mocked."
            };
        }

        public static ReportObject ExportAndStore(ReportObject report)
        {
            string fileName = $"Report_{report.PatientID}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = Path.Combine(Path.GetTempPath(), fileName);
            File.WriteAllText(filePath, report.SummaryText);
            report.FilePath = filePath;
            return report;
        }
    }
}