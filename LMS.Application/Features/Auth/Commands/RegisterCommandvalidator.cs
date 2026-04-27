using FluentValidation;
using LMS.Application.Features.Auth.Commands;

namespace LMS.Application.Features.Auth.Commands;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Request.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Request.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Request.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Must have at least one uppercase letter.")
            .Matches(@"[0-9]").WithMessage("Must have at least one digit.");
        RuleFor(x => x.Request.Role)
            .Must(r => new[] { "Student", "Instructor", "Admin" }.Contains(r))
            .WithMessage("Role must be Student, Instructor, or Admin.");
    }
}