using LMS.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace LMS.Infrastructure.Services;

public class StripeService : IStripeService
{
    private readonly IConfiguration _config;

    public StripeService(IConfiguration config)
    {
        _config = config;
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
    }

    public async Task<string> CreateCheckoutSessionAsync(
        Guid userId, Guid courseId, decimal amount,
        string courseTitle, CancellationToken ct = default)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "inr",
                        UnitAmount = (long)(amount * 100), // paise
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = courseTitle
                        }
                    },
                    Quantity = 1
                }
            },
            Mode = "payment",
            SuccessUrl = "http://localhost:4200/payment/success?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = "http://localhost:4200/payment/cancel",
            Metadata = new Dictionary<string, string>
            {
                { "userId",   userId.ToString() },
                { "courseId", courseId.ToString() }
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options, cancellationToken: ct);
        return session.Url; // redirect URL for frontend
    }

    public Task<bool> VerifyWebhookAsync(string payload, string signature)
    {
        try
        {
            var webhookSecret = _config["Stripe:WebhookSecret"]!;
            EventUtility.ConstructEvent(payload, signature, webhookSecret);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}