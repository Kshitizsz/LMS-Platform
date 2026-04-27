namespace LMS.Domain.Interfaces;

public interface INotificationService
{
    Task SendToUserAsync(string userId, string type, string message, CancellationToken ct = default);
    Task SendToCourseAsync(string courseId, string type, string message, CancellationToken ct = default);
    Task SendToAllAsync(string type, string message, CancellationToken ct = default);
}