using System.Security.Claims;
using LMS.Application.DTOs.Enrollment;
using LMS.Application.Features.Enrollments.Commands;
using LMS.Application.Features.Enrollments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class EnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public EnrollmentsController(IMediator mediator) => _mediator = mediator;

    //private Guid CurrentUserId =>
    //    Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private Guid CurrentUserId
    {
        get
        {
            // Try both claim types
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                   ?? User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(id))
                throw new UnauthorizedAccessException("User ID not found in token.");

            return Guid.Parse(id);
        }
    }
    /// <summary>Get my enrollments with progress</summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(List<EnrollmentDto>), 200)]
    public async Task<IActionResult> GetMine(CancellationToken ct)
        => Ok(await _mediator.Send(new GetMyEnrollmentsQuery(CurrentUserId), ct));

    /// <summary>Enroll in a course</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Enroll(
        [FromBody] EnrollRequest req, CancellationToken ct)
    {
        var id = await _mediator.Send(
            new EnrollCommand(CurrentUserId, req.CourseId), ct);
        return StatusCode(201, id);
    }
}