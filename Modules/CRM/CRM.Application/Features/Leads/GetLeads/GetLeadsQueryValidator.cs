using CRM.Application.Validation;
using CRM.Domain.Common;
using FluentValidation;

namespace CRM.Application.Features.Leads.GetLeads;

public sealed class GetLeadsQueryValidator : AbstractValidator<GetLeadsQuery>
{
    public GetLeadsQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();

        RuleFor(x => x.Search)
            .MaximumLength(InputSecurityLimits.MaxSearchLength)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Search contains unsafe content.")
            .When(x => !string.IsNullOrWhiteSpace(x.Search));

        RuleFor(x => x.Status)
            .MaximumLength(InputSecurityLimits.MaxStatusLength)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Status contains unsafe content.")
            .When(x => !string.IsNullOrWhiteSpace(x.Status));
    }
}
