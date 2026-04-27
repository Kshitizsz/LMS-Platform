namespace LMS.Application.DTOs.Auth;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Role = "Student");

public record LoginRequest(string Email, string Password);

public record AuthResponse(
    string Token,
    string Email,
    string FullName,
    string Role,
    DateTime ExpiresAt);