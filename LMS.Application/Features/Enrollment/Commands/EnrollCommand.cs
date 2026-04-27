using LMS.Application.Common.Exceptions;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using MediatR;

namespace LMS.Application.Features.Enrollments.Commands;

public record EnrollCommand(Guid UserId, Guid CourseId) : IRequest<Guid>;

public class EnrollCommandHandler : IRequestHandler<EnrollCommand, Guid>
{
    private readonly IUnitOfWork _uow;
    private readonly INotificationService _notifications;
    private readonly IEmailJobService _jobs;
    public EnrollCommandHandler(
        IUnitOfWork uow,
        INotificationService notifications,
         IEmailJobService jobs)
    {
        _uow = uow;
        _notifications = notifications;
        _jobs = jobs;
    }

    public async Task<Guid> Handle(EnrollCommand cmd, CancellationToken ct)
    {
        var existing = await _uow.Repository<Enrollment>()
            .FindAsync(e => e.UserId == cmd.UserId
                         && e.CourseId == cmd.CourseId, ct);

        if (existing.Any())
            throw new AppException("Already enrolled in this course.");

        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            CourseId = cmd.CourseId
        };

        await _uow.Repository<Enrollment>().AddAsync(enrollment, ct);
        await _uow.SaveChangesAsync(ct);

        // Fire real-time notification to the user
        await _notifications.SendToUserAsync(
            cmd.UserId.ToString(),
            "enrollment",
            "You have successfully enrolled in the course!",
            ct);

        var user = await _uow.Repository<User>().GetByIdAsync(cmd.UserId, ct);
        var course = await _uow.Repository<Course>().GetByIdAsync(cmd.CourseId, ct);

        if (user is not null && course is not null)
        {
            _jobs.QueueEnrollmentConfirmation(
                user.Email,
                $"{user.FirstName} {user.LastName}",
                course.Title);
        }

        return enrollment.Id;
    }
}