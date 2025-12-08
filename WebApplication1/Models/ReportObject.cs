using System;

namespace WebApplication1.Models
{
    public class ReportObject
    {
        public int PatientID { get; set; } = 0;
        public int ReportID { get; set; } = 0;
        public DateTime StartTime { get; set; } = DateTime.MinValue;
        public DateTime EndTime { get; set; } = DateTime.MinValue;
        public double MaxPressure { get; set; } = 0.0;
        public int TotalAlerts { get; set; } = 0;
        public string SummaryText { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
    }
}