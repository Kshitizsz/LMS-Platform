using LMS.Domain.Entities;

namespace LMS.Domain.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime Expiry) GenerateToken(User user);
    Guid? GetUserIdFromToken(string token);
}