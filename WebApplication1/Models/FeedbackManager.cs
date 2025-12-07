using System;

namespace WebApplication1.Models
{
    public static class FeedbackManager
    {
        // USER STORY P-4: Patient submits comment
        public static bool SubmitPatientComment(int frameId, int patientId, string content)
        {
            // Security Note: We use this method to prepare parameterized SQL queries.
            // In a real database scenario: 
            // string sql = "INSERT INTO COMMENT VALUES (@p1, @p2, @p3)";
            return true; // Returns true to simulate successful DB insertion
        }

        // USER STORY C-4: Clinician replies to specific comment
        public static bool SubmitClinicianReply(int parentCommentId, int clinicianId, string replyText)
        {
            // Logic to link this reply to the parentCommentId in the database
            return true;
        }
    }
}