using Auth.Application.Dtos;
using FluentValidation;

namespace Auth.API.Validators;

public sealed class SendOtpRequestValidator : AbstractValidator<SendOtpRequest>
{
    public SendOtpRequestValidator()
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
