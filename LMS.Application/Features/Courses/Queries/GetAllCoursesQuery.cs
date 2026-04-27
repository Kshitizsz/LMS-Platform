using LMS.Application.DTOs.Course;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Courses.Queries;

public record GetAllCoursesQuery(bool PublishedOnly = false) : IRequest<List<CourseDto>>;

public class GetAllCoursesQueryHandler
    : IRequestHandler<GetAllCoursesQuery, List<CourseDto>>
{
    private readonly IUnitOfWork _uow;

    public GetAllCoursesQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<List<CourseDto>> Handle(
        GetAllCoursesQuery qry, CancellationToken ct)
    {
        var courses = await _uow.Repository<Course>().GetAllAsync(ct);

        if (qry.PublishedOnly)
            courses = courses.Where(c => c.IsPublished);


        // Get instructor IDs
        var instructorIds = courses.Select(c => c.InstructorId).Distinct();
        var instructors = await _uow.Repository<User>()
            .FindAsync(u => instructorIds.Contains(u.Id), ct);

        return courses.Select(c =>
        {
            var instructor = instructors.FirstOrDefault(u => u.Id == c.InstructorId);
            return new CourseDto(
                c.Id, c.Title, c.Description, c.Price,
                c.ThumbnailUrl, c.IsPublished,
                instructor is not null
                    ? $"{instructor.FirstName} {instructor.LastName}"
                    : "Unknown",
                c.Modules?.Count ?? 0,
                c.Modules?.Sum(m => m.Lessons?.Count ?? 0) ?? 0,
                c.CreatedAt);
        }).ToList();
    }
}