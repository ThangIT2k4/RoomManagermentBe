using FluentValidation;

namespace Lease.Application.Features.Leases.CreateManual;

public sealed class CreateManualLeaseCommandValidator : AbstractValidator<CreateManualLeaseCommand>
{
    public CreateManualLeaseCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Mã người dùng không được để trống.");

        RuleFor(x => x.Request)
            .NotNull().WithMessage("Dữ liệu tạo hợp đồng không được để trống.");

        RuleFor(x => x.Request.UnitId)
            .NotEmpty().WithMessage("Mã phòng không được để trống.");

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
            .WithMessage("Ngày thanh toán phải trong khoảng 1-28.");

        RuleFor(x => x.Request)
            .Must(r => r.TenantUserId.HasValue || !string.IsNullOrWhiteSpace(r.TenantFullName))
            .WithMessage("Phải cung cấp tenant_user_id hoặc tenant_full_name.");

        RuleFor(x => x.Request.TenantFullName)
            .MaximumLength(255)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.TenantFullName))
            .WithMessage("Tên tenant không được vượt quá 255 ký tự.");
    }
}
