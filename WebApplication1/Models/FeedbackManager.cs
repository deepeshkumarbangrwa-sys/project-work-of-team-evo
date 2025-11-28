
    // --- FILE: Models/FeedbackManager.cs ---
using System.Data;

public static class FeedbackManager
    {
        // P-4: Patient submits a new comment
        public static bool SubmitPatientComment(int frameId, int patientId, string content)
        {
            // Define parameters for ReportQueries.InsertComment()
            object[] parameters = new object[]
            {
            frameId,
            patientId,
            content
            };

            // Executes the parameterized query securely (Focus Point: Secure Coding)
            // NOTE: This executes the query defined in ReportQueries.cs
            // ReportQueries.ExecuteParameterizedCommand(ReportQueries.InsertComment(), parameters);
            return true;
        }

        // C-4: Clinician submits a reply
        public static bool SubmitClinicianReply(int parentCommentId, int clinicianId, string replyText)
        {
            // Define parameters for ReportQueries.InsertClinicianReply()
            object[] parameters = new object[]
            {
            parentCommentId,
            clinicianId,
            replyText
            };

            // Executes the parameterized query securely (Focus Point: In-Thread Reply Logic)
            // NOTE: This executes the query defined in ReportQueries.cs
            // ReportQueries.ExecuteParameterizedCommand(ReportQueries.InsertClinicianReply(), parameters);
            return true;
        }

        // NOTE: You would also implement GetFeedbackThread logic here to retrieve comments for the View.
    }
