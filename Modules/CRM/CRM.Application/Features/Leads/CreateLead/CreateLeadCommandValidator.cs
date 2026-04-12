using CRM.Application.Validation;
using CRM.Domain.Common;
using FluentValidation;

namespace CRM.Application.Features.Leads.CreateLead;

public sealed class CreateLeadCommandValidator : AbstractValidator<CreateLeadRequest>
{
    public CreateLeadCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.FullName)
            .MaximumLength(InputSecurityLimits.MaxLeadNameLength)
            .WithMessage($"Họ và tên không được vượt quá {InputSecurityLimits.MaxLeadNameLength} ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Họ và tên chứa nội dung không an toàn.")
            .When(x => !string.IsNullOrWhiteSpace(x.FullName));

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Trạng thái không được để trống.")
            .MaximumLength(InputSecurityLimits.MaxStatusLength)
            .WithMessage($"Trạng thái không được vượt quá {InputSecurityLimits.MaxStatusLength} ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Trạng thái chứa nội dung không an toàn.");
    }
}
