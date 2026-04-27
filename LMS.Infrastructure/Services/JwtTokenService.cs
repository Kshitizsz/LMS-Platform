using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LMS.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config) => _config = config;

    public (string Token, DateTime Expiry) GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!));

        var expiry = DateTime.UtcNow.AddMinutes(
            double.Parse(_config["Jwt:ExpiryMinutes"]!));

        var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ← change Sub to this
    new Claim(JwtRegisteredClaimNames.Email, user.Email),
    new Claim(ClaimTypes.Role, user.Role),
    new Claim("fullName", $"{user.FirstName} {user.LastName}"),
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
};

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return (new JwtSecurityTokenHandler().WriteToken(token), expiry);
    }

    public Guid? GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var sub = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        return sub is null ? null : Guid.Parse(sub);
    }
}