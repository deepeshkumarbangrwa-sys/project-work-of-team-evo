namespace WebApplication1.Models
{
    
public class ReportObject
    {
        public int PatientID { get; set; }
        public int ReportID { get; set; } // PK when saved to DB
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double MaxPressure { get; set; }
        public int TotalAlerts { get; set; }
        public string SummaryText { get; set; }
        public string FilePath { get; set; }
    }
}
