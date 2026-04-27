namespace LMS.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public string StripeSessionId { get; set; } = string.Empty;
    public string StripePaymentIntentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "inr";
    public string Status { get; set; } = "Pending"; // Pending | Succeeded | Failed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
}