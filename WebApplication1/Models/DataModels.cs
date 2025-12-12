using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WebApplication1.Models // <-- Corrected Namespace
{
    // --- 1. User Model ---
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "Patient"; 

        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        public ICollection<SensorData> SensorDataEntries { get; set; } = new List<SensorData>();
        public ICollection<UserFeedback> FeedbackEntries { get; set; } = new List<UserFeedback>();
    }

    // --- 2. SensorData Model ---
    public class SensorData
    {
        [Key]
        public int DataId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } = default!;

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public int PeakPressureIndex { get; set; }

        [Required]
        public double ContactAreaPercentage { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string AlertStatus { get; set; } = "OK"; 
        
        [Required]
        [Column(TypeName = "ntext")]
        public string PressureMatrixJson { get; set; } = string.Empty;
    }

    // --- 3. UserFeedback Model ---
    public class UserFeedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } = default!;

        [Required]
        public DateTime Timestamp { get; set; } 

        [Required]
        [Column(TypeName = "ntext")]
        public string CommentText { get; set; } = string.Empty;

        [ForeignKey("SensorData")]
        public int? LinkedDataId { get; set; }
        public SensorData? LinkedData { get; set; }

        [Column(TypeName = "ntext")]
        public string ClinicianReply { get; set; } = string.Empty;
        
        public bool IsReviewed { get; set; } = false;
    }
}