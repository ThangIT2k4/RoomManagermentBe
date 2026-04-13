using FluentValidation;

namespace Lease.Application.Features.PaymentCycles.UpsertPaymentCycle;

public sealed class UpsertPaymentCycleCommandValidator : AbstractValidator<UpsertPaymentCycleCommand>
{
    public UpsertPaymentCycleCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Mã người dùng không được để trống.");

        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage("Tên chu kỳ thanh toán không được để trống.")
            .MaximumLength(255).WithMessage("Tên chu kỳ thanh toán không được vượt quá 255 ký tự.");

        RuleFor(x => x.Request.DurationMonths)
            .GreaterThan(0).WithMessage("Số tháng của chu kỳ phải lớn hơn 0.");

        RuleFor(x => x.Request.DayOfMonth)
            .InclusiveBetween(1, 28)
            .When(x => x.Request.DayOfMonth.HasValue)
            .WithMessage("Ngày trong tháng phải nằm trong khoảng 1-28.");
    }
}
