using CRM.Application.Validation;
using CRM.Domain.Common;
using FluentValidation;

namespace CRM.Application.Features.Leads.GetLeads;

public sealed class GetLeadsQueryValidator : AbstractValidator<GetLeadsQuery>
{
    public GetLeadsQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.Search)
            .MaximumLength(InputSecurityLimits.MaxSearchLength)
            .WithMessage($"Từ khóa tìm kiếm không được vượt quá {InputSecurityLimits.MaxSearchLength} ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Từ khóa tìm kiếm chứa nội dung không an toàn.")
            .When(x => !string.IsNullOrWhiteSpace(x.Search));

        RuleFor(x => x.Status)
            .MaximumLength(InputSecurityLimits.MaxStatusLength)
            .WithMessage($"Trạng thái không được vượt quá {InputSecurityLimits.MaxStatusLength} ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Trạng thái chứa nội dung không an toàn.")
            .When(x => !string.IsNullOrWhiteSpace(x.Status));
    }
}
