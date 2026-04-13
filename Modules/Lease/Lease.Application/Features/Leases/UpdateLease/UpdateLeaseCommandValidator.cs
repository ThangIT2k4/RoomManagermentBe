using FluentValidation;

namespace Lease.Application.Features.Leases.UpdateLease;

public sealed class UpdateLeaseCommandValidator : AbstractValidator<UpdateLeaseCommand>
{
    public UpdateLeaseCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Mã người dùng không được để trống.");

        RuleFor(x => x.Request.LeaseId)
            .NotEmpty().WithMessage("Mã hợp đồng không được để trống.");

        RuleFor(x => x.Request.EndDate)
            .NotEmpty().WithMessage("Ngày kết thúc không được để trống.");

        RuleFor(x => x.Request.PaymentDay)
            .InclusiveBetween(1, 28)
            .When(x => x.Request.PaymentDay.HasValue)
            .WithMessage("Ngày thanh toán phải trong khoảng 1-28.");

        RuleFor(x => x.Request.Notes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Notes))
            .WithMessage("Ghi chú không được vượt quá 2000 ký tự.");
    }
}
