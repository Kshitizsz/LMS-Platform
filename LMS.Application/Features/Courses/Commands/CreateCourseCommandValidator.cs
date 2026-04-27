using FluentValidation;

namespace LMS.Application.Features.Courses.Commands;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Request.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Request.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Request.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.InstructorId).NotEmpty();
    }
}