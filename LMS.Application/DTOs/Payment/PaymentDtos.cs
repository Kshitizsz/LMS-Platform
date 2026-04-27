namespace LMS.Application.DTOs.Payment;

public record CreateCheckoutRequest(Guid CourseId);

public record CheckoutResponse(string CheckoutUrl, string SessionId);

public record PaymentDto(
    Guid Id,
    string CourseTitle,
    decimal Amount,
    string Status,
    DateTime CreatedAt);