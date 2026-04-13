using FluentValidation;

namespace Lease.Application.Features.MasterLeases.TerminateMasterLease;

public sealed class TerminateMasterLeaseCommandValidator : AbstractValidator<TerminateMasterLeaseCommand>
{
    public TerminateMasterLeaseCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Mã người dùng không được để trống.");

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Mã master lease không được để trống.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Lý do chấm dứt không được để trống.")
            .MaximumLength(2000).WithMessage("Lý do chấm dứt không được vượt quá 2000 ký tự.");
    }
}
