using LMS.Domain.Interfaces;
using LMS.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace LMS.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hub;

    public NotificationService(IHubContext<NotificationHub> hub)
        => _hub = hub;

    public async Task SendToUserAsync(
        string userId, string type, string message, CancellationToken ct = default)
        => await _hub.Clients
            .Group($"user-{userId}")
            .SendAsync("ReceiveNotification", new { type, message }, ct);

    public async Task SendToCourseAsync(
        string courseId, string type, string message, CancellationToken ct = default)
        => await _hub.Clients
            .Group($"course-{courseId}")
            .SendAsync("ReceiveNotification", new { type, message }, ct);

    public async Task SendToAllAsync(
        string type, string message, CancellationToken ct = default)
        => await _hub.Clients.All
            .SendAsync("ReceiveNotification", new { type, message }, ct);
}