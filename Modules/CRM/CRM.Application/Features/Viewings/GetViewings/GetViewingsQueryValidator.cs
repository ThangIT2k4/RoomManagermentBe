using FluentValidation;

namespace CRM.Application.Features.Viewings.GetViewings;

public sealed class GetViewingsQueryValidator : AbstractValidator<GetViewingsQuery>
{
    public GetViewingsQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.LeadId)
            .NotEmpty()
            .WithMessage("Mã khách hàng tiềm năng không được để trống.")
            .When(x => x.LeadId.HasValue);

        RuleFor(x => x.AgentUserId)
            .NotEmpty()
            .WithMessage("Mã nhân viên dẫn xem không được để trống.")
            .When(x => x.AgentUserId.HasValue);

        RuleFor(x => x)
            .Must(x => !x.From.HasValue || !x.To.HasValue || x.From.Value <= x.To.Value)
            .WithMessage("Thời gian bắt đầu không được lớn hơn thời gian kết thúc.")
            .When(x => x.From.HasValue && x.To.HasValue);

        RuleFor(x => x.Paging!)
            .SetValidator(new CRM.Application.Features.Shared.PagingRequestValidator())
            .When(x => x.Paging is not null);
    }
}

