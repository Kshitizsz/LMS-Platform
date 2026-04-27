using LMS.Application.Common.Exceptions;
using LMS.Application.DTOs.Auth;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using MediatR;

namespace LMS.Application.Features.Auth.Queries;

public record LoginQuery(LoginRequest Request) : IRequest<AuthResponse>;

public class LoginQueryHandler : IRequestHandler<LoginQuery, AuthResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtTokenService _jwt;

    public LoginQueryHandler(IUnitOfWork uow, IJwtTokenService jwt)
    {
        _uow = uow;
        _jwt = jwt;
    }

    public async Task<AuthResponse> Handle(LoginQuery qry, CancellationToken ct)
    {
        var users = await _uow.Repository<User>()
            .FindAsync(u => u.Email == qry.Request.Email, ct);

        var user = users.FirstOrDefault()
            ?? throw new UnauthorizedException("Invalid credentials.");

        if (!BCrypt.Net.BCrypt.Verify(qry.Request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials.");

        var (token, expiry) = _jwt.GenerateToken(user);
        return new AuthResponse(token, user.Email,
            $"{user.FirstName} {user.LastName}", user.Role, expiry);
    }
}