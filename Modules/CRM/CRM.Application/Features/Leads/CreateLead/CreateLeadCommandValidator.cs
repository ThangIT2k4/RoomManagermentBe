using CRM.Application.Validation;
using CRM.Domain.Common;
using FluentValidation;

namespace CRM.Application.Features.Leads.CreateLead;

public sealed class CreateLeadCommandValidator : AbstractValidator<CreateLeadRequest>
{
    public CreateLeadCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();

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
