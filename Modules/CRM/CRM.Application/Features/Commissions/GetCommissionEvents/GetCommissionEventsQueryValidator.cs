using CRM.Application.Features.Shared;
using CRM.Application.Validation;
using FluentValidation;

namespace CRM.Application.Features.Commissions.GetCommissionEvents;

public sealed class GetCommissionEventsQueryValidator : AbstractValidator<GetCommissionEventsQuery>
{
    public GetCommissionEventsQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.AgentUserId)
            .NotEmpty()
            .WithMessage("Mã nhân viên môi giới không được để trống.")
            .When(x => x.AgentUserId.HasValue);

        RuleFor(x => x.Status)
            .MaximumLength(50)
            .WithMessage("Trạng thái không được vượt quá 50 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Trạng thái chứa nội dung không an toàn.")
            .When(x => !string.IsNullOrWhiteSpace(x.Status));

        RuleFor(x => x.Paging!)
            .SetValidator(new PagingRequestValidator())
            .When(x => x.Paging is not null);
    }
}


