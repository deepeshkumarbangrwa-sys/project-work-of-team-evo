// --- FILE: Models/ReportGenerator.cs ---
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

public static class ReportGenerator
{
    // Method to calculate metrics and compile the report object (C-5)
    public static ReportObject GenerateReportData(int patientId, DateTime start, DateTime end)
    {
        // 1. Prepare parameters for the DAL call
        object[] parameters = new object[] { patientId, start, end };

        // 2. Mock Data Retrieval (Replace with actual DAL logic later)
        // In a real app, this retrieves the raw data using ReportQueries.GetReportData()
        DataTable dt = new DataTable();

        // --- Core Analysis Logic (C-5) ---
        double maxPressure = 0;
        int totalAlerts = 0;

        // NOTE: Placeholder logic - In a real system, you would iterate over 'dt' 
        // to calculate actual values based on the data.
        if (dt.Rows.Count > 0)
        {
            // Example of how you would calculate metrics from the DataTable:
            // maxPressure = dt.AsEnumerable().Max(row => row.Field<int>("PeakPressureIndex")); 
            // totalAlerts = dt.AsEnumerable().Count(row => !row.IsNull("AlertID"));
        }

        // Use fixed mock data for compilation:
        maxPressure = 180.5;
        totalAlerts = 3;

        string summary = $"Max Pressure Recorded: {maxPressure}mmHg. Total High-Risk Alerts: {totalAlerts}.";

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

    // Method for PDF Export and DB Metadata Storage (A-3)
    public static ReportObject ExportAndStore(ReportObject report)
    {
        // --- This section demonstrates the logic for File Creation and DB Update (A-3) ---

        // 1. Create a secure, timestamped file name
        string fileName = $"Report_{report.PatientID}_{report.StartTime:yyyyMMdd_HHmmss}.pdf";
        string filePath = $@"C:\GrapheneTrace\Reports\{fileName}";

        // 2. **PDF GENERATION LOGIC** // 
        // (The iTextSharp/PDFsharp code to write content to filePath goes here.)

        // 3. Store the record to the REPORT table and retrieve the new PK (ReportID)
        // NOTE: This logic assumes a DAL method exists to execute the ReportQueries.InsertReport()
        // object[] parameters = new object[] { report.PatientID, report.StartTime, report.EndTime, report.SummaryText, filePath };
        // report.ReportID = DAL.ExecuteScalar(ReportQueries.InsertReport(), parameters); 
        report.ReportID = 1; // Mock ID assignment for compilation

        report.FilePath = filePath;

        return report;
    }
}