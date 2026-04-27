using LMS.Application.Common.Exceptions;
using LMS.Application.DTOs.Course;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using MediatR;

namespace LMS.Application.Features.Courses.Queries;

public record GetCourseByIdQuery(Guid Id) : IRequest<CourseDto>;

public class GetCourseByIdQueryHandler
    : IRequestHandler<GetCourseByIdQuery, CourseDto>
{
    private readonly IUnitOfWork _uow;

    public GetCourseByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<CourseDto> Handle(
        GetCourseByIdQuery qry, CancellationToken ct)
    {
        var course = await _uow.Repository<Course>().GetByIdAsync(qry.Id, ct)
            ?? throw new NotFoundException(nameof(Course), qry.Id);

        return new CourseDto(
            course.Id,
            course.Title,
            course.Description,
            course.Price,
            course.ThumbnailUrl,
            course.IsPublished,
            course.Instructor is not null
                ? $"{course.Instructor.FirstName} {course.Instructor.LastName}"
                : "Unknown",
            course.Modules?.Count ?? 0,
            course.Modules?.Sum(m => m.Lessons?.Count ?? 0) ?? 0,
            course.CreatedAt);
    }
}