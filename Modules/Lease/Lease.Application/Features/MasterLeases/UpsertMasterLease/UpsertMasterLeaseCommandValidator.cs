using FluentValidation;

namespace Lease.Application.Features.MasterLeases.UpsertMasterLease;

public sealed class UpsertMasterLeaseCommandValidator : AbstractValidator<UpsertMasterLeaseCommand>
{
    public UpsertMasterLeaseCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Mã người dùng không được để trống.");

        RuleFor(x => x.Request.LandlordUserId)
            .NotEmpty().WithMessage("Mã chủ nhà không được để trống.");

        RuleFor(x => x.Request.PropertyId)
            .NotEmpty().WithMessage("Mã bất động sản không được để trống.");

        RuleFor(x => x.Request.EndDate)
            .GreaterThan(x => x.Request.StartDate)
            .WithMessage("Ngày kết thúc phải lớn hơn ngày bắt đầu.");

        RuleFor(x => x.Request.RentAmount)
            .GreaterThan(0).WithMessage("Tiền thuê phải lớn hơn 0.");

        RuleFor(x => x.Request.DepositAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Request.DepositAmount.HasValue)
            .WithMessage("Tiền cọc không được âm.");

        RuleFor(x => x.Request.PaymentDay)
            .InclusiveBetween(1, 28)
            .When(x => x.Request.PaymentDay.HasValue)
            .WithMessage("Ngày thanh toán phải nằm trong khoảng 1-28.");

        RuleFor(x => x.Request.Notes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Notes))
            .WithMessage("Ghi chú không được vượt quá 2000 ký tự.");
    }
}
