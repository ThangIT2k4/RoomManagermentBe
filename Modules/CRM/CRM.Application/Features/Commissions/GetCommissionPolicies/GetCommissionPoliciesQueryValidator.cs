using CRM.Application.Validation;
using FluentValidation;

namespace CRM.Application.Features.Commissions.GetCommissionPolicies;

public sealed class GetCommissionPoliciesQueryValidator : AbstractValidator<GetCommissionPoliciesQuery>
{
    public GetCommissionPoliciesQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Từ khóa tìm kiếm không được vượt quá 100 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Từ khóa tìm kiếm chứa nội dung không an toàn.")
            .When(x => !string.IsNullOrWhiteSpace(x.Search));

        RuleFor(x => x.Paging!)
            .SetValidator(new CRM.Application.Features.Shared.PagingRequestValidator())
            .When(x => x.Paging is not null);
    }
}

