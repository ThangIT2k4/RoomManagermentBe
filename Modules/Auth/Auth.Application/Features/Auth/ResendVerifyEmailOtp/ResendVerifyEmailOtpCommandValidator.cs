using FluentValidation;

namespace Auth.Application.Features.Auth.ResendVerifyEmailOtp;

public sealed class ResendVerifyEmailOtpCommandValidator : AbstractValidator<ResendVerifyEmailOtpCommand>
{
    public ResendVerifyEmailOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
