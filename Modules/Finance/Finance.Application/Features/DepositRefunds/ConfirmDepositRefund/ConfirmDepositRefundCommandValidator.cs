using FluentValidation;

namespace Finance.Application.Features.DepositRefunds.ConfirmDepositRefund;

public sealed class ConfirmDepositRefundCommandValidator : AbstractValidator<ConfirmDepositRefundCommand>
{
    public ConfirmDepositRefundCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.RefundId)
            .NotEmpty()
            .WithMessage("Mã yêu cầu hoàn cọc không được để trống.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Mã người dùng không được để trống.");

        RuleFor(x => x.PaidAtUtc)
            .NotEmpty()
            .WithMessage("Thời điểm thanh toán không được để trống.")
            .Must(date => date <= DateTime.UtcNow.AddDays(1))
            .WithMessage("Thời điểm thanh toán không được vượt quá hiện tại 1 ngày.");

        RuleFor(x => x.ReferenceCode)
            .MaximumLength(100)
            .WithMessage("Mã tham chiếu không được vượt quá 100 ký tự.")
            .When(x => !string.IsNullOrWhiteSpace(x.ReferenceCode));
    }
}

