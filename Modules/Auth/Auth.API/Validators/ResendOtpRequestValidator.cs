using Auth.Application.Dtos;
using FluentValidation;

namespace Auth.API.Validators;

public sealed class ResendOtpRequestValidator : AbstractValidator<ResendOtpRequest>
{
    public ResendOtpRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Purpose)
            .IsInEnum();
    }
}
