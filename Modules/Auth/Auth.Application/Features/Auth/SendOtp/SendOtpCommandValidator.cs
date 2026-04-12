using Auth.Application.Validation;
using FluentValidation;

namespace Auth.Application.Features.Auth.SendOtp;

public sealed class SendOtpCommandValidator : AbstractValidator<SendOtpCommand>
{
    public SendOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(120)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Email contains unsafe content.");

        RuleFor(x => x.Purpose)
            .IsInEnum();
    }
}
