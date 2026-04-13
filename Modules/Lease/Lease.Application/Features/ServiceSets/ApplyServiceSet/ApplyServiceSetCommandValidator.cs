using FluentValidation;

namespace Lease.Application.Features.ServiceSets.ApplyServiceSet;

public sealed class ApplyServiceSetCommandValidator : AbstractValidator<ApplyServiceSetCommand>
{
    public ApplyServiceSetCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Mã người dùng không được để trống.");

        RuleFor(x => x.Request.LeaseId)
            .NotEmpty().WithMessage("Mã hợp đồng không được để trống.");

        RuleFor(x => x.Request.LeaseServiceSetId)
            .NotEmpty().WithMessage("Mã bộ dịch vụ không được để trống.");
    }
}
