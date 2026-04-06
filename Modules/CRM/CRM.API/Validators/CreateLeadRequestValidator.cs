using CRM.Application.Features.Leads;
using CRM.Domain.Common;
using FluentValidation;

namespace CRM.API.Validators;

public sealed class CreateLeadRequestValidator : AbstractValidator<CreateLeadRequest>
{
    public CreateLeadRequestValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.FullName)
            .MaximumLength(InputSecurityLimits.MaxLeadNameLength)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("FullName contains unsafe content.")
            .When(x => !string.IsNullOrWhiteSpace(x.FullName));

        RuleFor(x => x.Status)
            .NotEmpty()
            .MaximumLength(InputSecurityLimits.MaxStatusLength)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Status contains unsafe content.");
    }
}
