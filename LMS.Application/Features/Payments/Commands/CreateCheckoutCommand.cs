using LMS.Application.Common.Exceptions;
using LMS.Application.DTOs.Payment;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using MediatR;

namespace LMS.Application.Features.Payments.Commands;

public record CreateCheckoutCommand(Guid UserId, Guid CourseId)
    : IRequest<CheckoutResponse>;

public class CreateCheckoutCommandHandler
    : IRequestHandler<CreateCheckoutCommand, CheckoutResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IStripeService _stripe;

    public CreateCheckoutCommandHandler(IUnitOfWork uow, IStripeService stripe)
    {
        _uow = uow;
        _stripe = stripe;
    }

    public async Task<CheckoutResponse> Handle(
        CreateCheckoutCommand cmd, CancellationToken ct)
    {
        var course = await _uow.Repository<Course>().GetByIdAsync(cmd.CourseId, ct)
            ?? throw new NotFoundException(nameof(Course), cmd.CourseId);

        var checkoutUrl = await _stripe.CreateCheckoutSessionAsync(
            cmd.UserId, cmd.CourseId, course.Price, course.Title, ct);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            CourseId = cmd.CourseId,
            Amount = course.Price,
            StripeSessionId = checkoutUrl,
            Status = "Pending"
        };

        await _uow.Repository<Payment>().AddAsync(payment, ct);
        await _uow.SaveChangesAsync(ct);

        return new CheckoutResponse(checkoutUrl, payment.StripeSessionId);
    }
}