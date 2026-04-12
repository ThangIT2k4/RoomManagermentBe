using Auth.Application.Validation;
using FluentValidation;

namespace Auth.Application.Features.Auth.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(120)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Login contains unsafe content.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(128);
    }
}
