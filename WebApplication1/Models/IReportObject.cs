// --- FILE: Models/ReportObject.cs ---

public interface IReportObject
{
    DateTime EndTime { get; set; }
    string FilePath { get; set; }
    double MaxPressure { get; set; }
    int PatientID { get; set; }
    int ReportID { get; set; }
    DateTime StartTime { get; set; }
    string SummaryText { get; set; }
    int TotalAlerts { get; set; }
}