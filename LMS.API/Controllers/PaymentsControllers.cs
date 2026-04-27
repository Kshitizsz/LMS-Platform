using System.Security.Claims;
using LMS.Application.DTOs.Payment;
using LMS.Application.Features.Payments.Commands;
using LMS.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStripeService _stripe;

    public PaymentsController(IMediator mediator, IStripeService stripe)
    {
        _mediator = mediator;
        _stripe = stripe;
    }

    /// <summary>Create Stripe checkout session for a course</summary>
    [HttpPost("checkout")]
    [Authorize]
    [ProducesResponseType(typeof(CheckoutResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Checkout(
        [FromBody] CreateCheckoutRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(
            new CreateCheckoutCommand(userId, req.CourseId), ct);
        return Ok(result);
    }

    /// <summary>Stripe webhook — auto-enroll on payment success</summary>
    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> Webhook()
    {
        var payload = await new StreamReader(Request.Body).ReadToEndAsync();
        var sig = Request.Headers["Stripe-Signature"].ToString();

        var isValid = await _stripe.VerifyWebhookAsync(payload, sig);
        if (!isValid) return BadRequest("Invalid webhook signature.");

        // TODO: parse event, auto-enroll user on checkout.session.completed
        return Ok();
    }
}