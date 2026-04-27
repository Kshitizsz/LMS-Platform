using LMS.Application.Common.Exceptions;
using LMS.Application.DTOs.Course;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using MediatR;

namespace LMS.Application.Features.Courses.Commands;

public record CreateCourseCommand(CreateCourseRequest Request, Guid InstructorId)
    : IRequest<Guid>;

public class CreateCourseCommandHandler
    : IRequestHandler<CreateCourseCommand, Guid>
{
    private readonly IUnitOfWork _uow;

    public CreateCourseCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Guid> Handle(CreateCourseCommand cmd, CancellationToken ct)
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = cmd.Request.Title,
            Description = cmd.Request.Description,
            Price = cmd.Request.Price,
            ThumbnailUrl = cmd.Request.ThumbnailUrl,
            InstructorId = cmd.InstructorId
        };

        await _uow.Repository<Course>().AddAsync(course, ct);
        await _uow.SaveChangesAsync(ct);
        return course.Id;
    }
}