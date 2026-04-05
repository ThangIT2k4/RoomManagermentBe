using Auth.Application.Dtos;
using FluentValidation;

namespace Auth.API.Validators;

public sealed class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(120)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Email contains unsafe content.");

        RuleFor(x => x.OtpCode)
            .NotEmpty()
            .Length(6)
            .Matches("^[0-9]{6}$");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128);
    }
}
