using Auth.Application.Validation;
using FluentValidation;

namespace Auth.Application.Features.Register;

public sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Full name contains unsafe content.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.Username)
            .MaximumLength(50)
            .Matches("^[a-zA-Z0-9._-]+$")
            .WithMessage("Username only allows letters, digits, dot, underscore and dash.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Username contains unsafe content.")
            .When(x => !string.IsNullOrWhiteSpace(x.Username));

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .Matches("^[0-9+()\\-\\s]{7,20}$")
            .WithMessage("Phone number format is invalid.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }
}
