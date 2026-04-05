using Auth.Application.Dtos;
using FluentValidation;

namespace Auth.API.Validators;

public sealed class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(120)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Email contains unsafe content.");
    }
}
