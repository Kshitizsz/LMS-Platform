namespace LMS.Domain.Interfaces;

public interface IEmailJobService
{
    void QueueEnrollmentConfirmation(string email, string fullName, string courseTitle);
    void QueueCourseCompletionEmail(string email, string fullName, string courseTitle);
    void ScheduleWeeklySummary(string email, string fullName);
}