using LMS.Application.DTOs.Course;
using LMS.Application.Features.Courses.Commands;
using LMS.Application.Features.Courses.Queries;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _uow;
    public CoursesController(IMediator mediator, IUnitOfWork uow)
    {
        _mediator = mediator;
        _uow = uow;
    }

    /// <summary>Get all published courses</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CourseDto>), 200)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetAllCoursesQuery(), ct));

    /// <summary>Get a specific course by ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CourseDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetCourseByIdQuery(id), ct));

    /// <summary>Create a new course (Instructor only)</summary>
    [HttpPost]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCourseRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var id = await _mediator.Send(new CreateCourseCommand(req, userId), ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>Publish a course (Instructor only)</summary>
    [HttpPatch("{id:guid}/publish")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        var course = await _uow.Repository<Course>().GetByIdAsync(id, ct);
        if (course is null) return NotFound();

        course.IsPublished = true;
        _uow.Repository<Course>().Update(course);
        await _uow.SaveChangesAsync(ct);
        return NoContent();
    }
}