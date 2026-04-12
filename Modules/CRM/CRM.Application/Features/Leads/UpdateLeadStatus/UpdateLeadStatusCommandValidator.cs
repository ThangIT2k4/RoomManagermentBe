using CRM.Application.Validation;
using CRM.Domain.Common;
using FluentValidation;

namespace CRM.Application.Features.Leads.UpdateLeadStatus;

public sealed class UpdateLeadStatusCommandValidator : AbstractValidator<UpdateLeadStatusRequest>
{
    public UpdateLeadStatusCommandValidator()
    {
        RuleFor(x => x.LeadId)
            .NotEmpty()
            .WithMessage("Mã khách hàng tiềm năng không được để trống.");

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Trạng thái không được để trống.")
            .MaximumLength(InputSecurityLimits.MaxStatusLength)
            .WithMessage($"Trạng thái không được vượt quá {InputSecurityLimits.MaxStatusLength} ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Trạng thái chứa nội dung không an toàn.");
    }
}
