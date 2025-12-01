// --- FILE: Models/ReportQueries.cs (FINAL VERSION) ---

using System.Data;
using System.Collections.Generic;


public static class ReportQueries
{
    // CITE NOTE: Parameterized queries prevent SQL injection and allow special characters.

    // 1. QUERY: INSERT a new Patient Comment (P-4) - Parameterized Query
    public static string InsertComment()
    {
        return "INSERT INTO COMMENT (FrameID, PatientID, Timestamp, CommentText) " +
               "VALUES (@frameID, @patientID, GETDATE(), @content)";
    }

    // 2. QUERY: INSERT a new Clinician Reply (C-4) - Parameterized Query
    public static string InsertClinicianReply()
    {
        return "INSERT INTO CLINICIAN_REPLY (CommentID, ClinicianID, Timestamp, ReplyText) " +
               "VALUES (@commentID, @clinicianID, GETDATE(), @replyText)";
    }

    // 3. QUERY: INSERT the final Report Metadata (A-3)
    public static string InsertReport()
    {
        return "INSERT INTO REPORT (PatientID, StartTime, EndTime, Summary, FilePath) " +
               "OUTPUT INSERTED.ReportID VALUES (@patientID, @start, @end, @summary, @filePath)";
    }

    // 4. QUERY: SELECT Frames and Alerts for Report Generation (C-5)
    public static string GetReportData()
    {
        return "SELECT T1.PeakPressureIndex, T1.ContactAreaPercentage, " +
               "T2.AlertID FROM PRESSURE_FRAME T1 " +
               "LEFT JOIN ALERT T2 ON T1.FrameID = T2.FrameID " +
               "WHERE T1.PatientID = @patientID AND T1.Timestamp BETWEEN @start AND @end";
    }
}