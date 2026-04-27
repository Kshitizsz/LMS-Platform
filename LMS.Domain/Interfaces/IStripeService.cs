namespace LMS.Domain.Interfaces;

public interface IStripeService
{
    Task<string> CreateCheckoutSessionAsync(Guid userId, Guid courseId, decimal amount, string courseTitle, CancellationToken ct = default);
    Task<bool> VerifyWebhookAsync(string payload, string signature);
}