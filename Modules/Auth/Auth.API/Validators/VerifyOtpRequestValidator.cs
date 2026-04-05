using Auth.Application.Dtos;
using FluentValidation;

namespace Auth.API.Validators;

public sealed class VerifyOtpRequestValidator : AbstractValidator<VerifyOtpRequest>
{
    public VerifyOtpRequestValidator()
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
