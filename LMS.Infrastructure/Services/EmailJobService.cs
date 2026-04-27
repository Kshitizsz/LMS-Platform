using Hangfire;
using LMS.Domain.Interfaces;

namespace LMS.Infrastructure.Services;

public class EmailJobService : IEmailJobService
{
    private readonly IEmailService _email;

    public EmailJobService(IEmailService email) => _email = email;

    public void QueueEnrollmentConfirmation(
        string email, string fullName, string courseTitle)
    {
        BackgroundJob.Enqueue(() =>
            SendEnrollmentEmail(email, fullName, courseTitle));
    }

    public void QueueCourseCompletionEmail(
        string email, string fullName, string courseTitle)
    {
        BackgroundJob.Enqueue(() =>
            SendCompletionEmail(email, fullName, courseTitle));
    }

    public void ScheduleWeeklySummary(string email, string fullName)
    {
        RecurringJob.AddOrUpdate(
            $"weekly-summary-{email}",
            () => SendWeeklySummary(email, fullName),
            Cron.Weekly(DayOfWeek.Monday, 9));
    }

    public async Task SendEnrollmentEmail(
        string email, string fullName, string courseTitle)
    {
        try
        {
            var html = $"""
                <h2>Welcome to {courseTitle}!</h2>
                <p>Hi {fullName}, you have successfully enrolled!</p>
                """;
            await _email.SendAsync(email, $"Enrolled: {courseTitle}", html);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Email skipped] {ex.Message}");
        }
    }

    public async Task SendCompletionEmail(
        string email, string fullName, string courseTitle)
    {
        try
        {
            var html = $"""
                <h2>Congratulations!</h2>
                <p>Hi {fullName}, you completed <strong>{courseTitle}</strong>! 🎉</p>
                """;
            await _email.SendAsync(email, $"Completed: {courseTitle}", html);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Email skipped] {ex.Message}");
        }
    }

    public async Task SendWeeklySummary(string email, string fullName)
    {
        try
        {
            var html = $"""
                <h2>Your Weekly Summary</h2>
                <p>Hi {fullName}, keep up the learning momentum!</p>
                """;
            await _email.SendAsync(email, "Your Weekly LMS Summary", html);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Email skipped] {ex.Message}");
        }
    }
}