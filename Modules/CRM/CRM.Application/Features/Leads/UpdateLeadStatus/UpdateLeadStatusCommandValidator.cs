using CRM.Application.Validation;
using CRM.Domain.Common;
using FluentValidation;

namespace CRM.Application.Features.Leads.UpdateLeadStatus;

public sealed class UpdateLeadStatusCommandValidator : AbstractValidator<UpdateLeadStatusRequest>
{
    public UpdateLeadStatusCommandValidator()
    {
        RuleFor(x => x.LeadId).NotEmpty();

        RuleFor(x => x.Status)
            .NotEmpty()
            .MaximumLength(InputSecurityLimits.MaxStatusLength)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Status contains unsafe content.");
    }
}
