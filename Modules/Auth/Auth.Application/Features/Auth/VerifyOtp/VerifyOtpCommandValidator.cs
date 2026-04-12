using Auth.Application.Validation;
using FluentValidation;

namespace Auth.Application.Features.Auth.VerifyOtp;

public sealed class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(120)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Email contains unsafe content.");

        RuleFor(x => x.Purpose)
            .IsInEnum();

        RuleFor(x => x.OtpCode)
            .NotEmpty()
            .Length(6)
            .Matches("^[0-9]{6}$");
    }
}
