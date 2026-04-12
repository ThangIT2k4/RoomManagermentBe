using Auth.Application.Validation;
using FluentValidation;

namespace Auth.Application.Features.Auth.Profile.UpdatePersonalInfo;

public sealed class UpdatePersonalInfoCommandValidator : AbstractValidator<UpdatePersonalInfoCommand>
{
    public UpdatePersonalInfoCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.FullName)
            .MaximumLength(120)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("FullName contains unsafe content.")
            .When(x => !string.IsNullOrWhiteSpace(x.FullName));

        RuleFor(x => x.Address)
            .MaximumLength(255)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Address contains unsafe content.")
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.Note)
            .MaximumLength(500)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Note contains unsafe content.")
            .When(x => !string.IsNullOrWhiteSpace(x.Note));
    }
}
