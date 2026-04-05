using Auth.API.Requests;
using FluentValidation;

namespace Auth.API.Validators;

public sealed class LoginApiRequestValidator : AbstractValidator<LoginApiRequest>
{
    public LoginApiRequestValidator()
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
