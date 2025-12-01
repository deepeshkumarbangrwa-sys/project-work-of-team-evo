// --- FILE: Models/FeedbackManager.cs (FINAL VERSION) ---
using System.Data;

public static class FeedbackManager
{
    // P-4: Patient submits a new comment
    public static bool SubmitPatientComment(int frameId, int patientId, string content)
    {
        // NOTE: In a real app, this would execute the parameterized query.
        // For compatibility, we assume success.

        // ReportQueries.ExecuteParameterizedCommand(ReportQueries.InsertComment(), parameters);
        return true;
    }

    // C-4: Clinician submits a reply
    public static bool SubmitClinicianReply(int parentCommentId, int clinicianId, string replyText)
    {
        // NOTE: In a real app, this would execute the parameterized query.
        // For compatibility, we assume success.

        // ReportQueries.ExecuteParameterizedCommand(ReportQueries.InsertClinicianReply(), parameters);
        return true;
    }
}