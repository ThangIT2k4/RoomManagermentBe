using CRM.Application.Features.Leads;
using CRM.Domain.Common;
using FluentValidation;

namespace CRM.API.Validators;

public sealed class UpdateLeadStatusRequestValidator : AbstractValidator<UpdateLeadStatusRequest>
{
    public UpdateLeadStatusRequestValidator()
    {
        RuleFor(x => x.LeadId)
            .NotEmpty();

        RuleFor(x => x.Status)
            .NotEmpty()
            .MaximumLength(InputSecurityLimits.MaxStatusLength)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Status contains unsafe content.");
    }
}
