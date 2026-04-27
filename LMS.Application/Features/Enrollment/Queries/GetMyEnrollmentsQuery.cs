using LMS.Application.DTOs.Enrollment;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using MediatR;

namespace LMS.Application.Features.Enrollments.Queries;

public record GetMyEnrollmentsQuery(Guid UserId) : IRequest<List<EnrollmentDto>>;

public class GetMyEnrollmentsQueryHandler
    : IRequestHandler<GetMyEnrollmentsQuery, List<EnrollmentDto>>
{
    private readonly IUnitOfWork _uow;

    public GetMyEnrollmentsQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<List<EnrollmentDto>> Handle(
    GetMyEnrollmentsQuery qry, CancellationToken ct)
    {
        // Get enrollments for this user
        var enrollments = (await _uow.Repository<Enrollment>()
            .FindAsync(e => e.UserId == qry.UserId, ct)).ToList();

        if (!enrollments.Any())
            return new List<EnrollmentDto>();

        // Load related courses
        var courseIds = enrollments.Select(e => e.CourseId).ToList();
        var courses = (await _uow.Repository<Course>()
            .FindAsync(c => courseIds.Contains(c.Id), ct)).ToList();

        return enrollments.Select(e =>
        {
            var course = courses.FirstOrDefault(c => c.Id == e.CourseId);
            return new EnrollmentDto(
                e.Id,
                e.CourseId,
                course?.Title ?? "Unknown Course",
                e.IsCompleted,
                e.EnrolledAt,
                0 // progress always 0 until lessons added
            );
        }).ToList();
    }
}