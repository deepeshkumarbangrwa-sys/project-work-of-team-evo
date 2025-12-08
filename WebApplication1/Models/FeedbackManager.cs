namespace WebApplication1.Models
{
    public static class FeedbackManager
    {
        // P-4: Submit Comment Logic
        public static bool SubmitPatientComment(int frameId, int patientId, string content)
        {
            // In a real app, SQL INSERT goes here.
            return true;
        }

        // C-4: Submit Reply Logic
        public static bool SubmitClinicianReply(int parentCommentId, int clinicianId, string replyText)
        {
            // In a real app, SQL INSERT goes here.
            return true;
        }
    }
}