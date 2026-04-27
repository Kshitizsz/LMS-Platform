using LMS.Application.Common.Exceptions;
using LMS.Application.DTOs.Auth;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;

using MediatR;

namespace LMS.Application.Features.Auth.Commands;

public record RegisterCommand(RegisterRequest Request) : IRequest<AuthResponse>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtTokenService _jwt;

    public RegisterCommandHandler(IUnitOfWork uow, IJwtTokenService jwt)
    {
        _uow = uow;
        _jwt = jwt;
    }

    public async Task<AuthResponse> Handle(RegisterCommand cmd, CancellationToken ct)
    {
        var repo = _uow.Repository<User>();
        var existing = await repo.FindAsync(u => u.Email == cmd.Request.Email, ct);

        if (existing.Any())
            throw new AppException("Email is already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = cmd.Request.FirstName,
            LastName = cmd.Request.LastName,
            Email = cmd.Request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(cmd.Request.Password),
            Role = cmd.Request.Role
        };

        await repo.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        var (token, expiry) = _jwt.GenerateToken(user);
        return new AuthResponse(token, user.Email,
            $"{user.FirstName} {user.LastName}", user.Role, expiry);
    }
}