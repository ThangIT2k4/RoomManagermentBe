using FluentValidation;

namespace Lease.Application.Features.Leases.TerminateLease;

public sealed class TerminateLeaseCommandValidator : AbstractValidator<TerminateLeaseCommand>
{
    public TerminateLeaseCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Mã người dùng không được để trống.");

        RuleFor(x => x.Request.LeaseId)
            .NotEmpty().WithMessage("Mã hợp đồng không được để trống.");

        RuleFor(x => x.Request.Reason)
            .NotEmpty().WithMessage("Lý do chấm dứt không được để trống.")
            .MinimumLength(10).WithMessage("Lý do chấm dứt phải có ít nhất 10 ký tự.")
            .MaximumLength(2000).WithMessage("Lý do chấm dứt không được vượt quá 2000 ký tự.");

        RuleFor(x => x.Request.RefundAmount)
            .NotNull().WithMessage("Số tiền hoàn cọc là bắt buộc khi bật create_refund.")
            .GreaterThanOrEqualTo(0).WithMessage("Số tiền hoàn cọc không được âm.")
            .When(x => x.Request.CreateRefund);
    }
}
