using FluentValidation;

namespace Finance.Application.Features.Payments.InitiateOnlinePayment;

public sealed class InitiateOnlinePaymentCommandValidator : AbstractValidator<InitiateOnlinePaymentCommand>
{
    private static readonly string[] AllowedMethodKeys = ["sepay", "vnpay", "momo"];

    public InitiateOnlinePaymentCommandValidator()
    {
        RuleFor(x => x.TenantUserId)
            .NotEmpty()
            .WithMessage("Mã người dùng tenant không được để trống.");

        RuleFor(x => x.InvoiceId)
            .NotEmpty()
            .WithMessage("Mã hóa đơn không được để trống.");

        RuleFor(x => x.MethodKey)
            .NotEmpty()
            .WithMessage("Phương thức thanh toán không được để trống.")
            .Must(IsAllowedMethodKey)
            .WithMessage("Phương thức thanh toán không hợp lệ.");
    }

    private static bool IsAllowedMethodKey(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim().ToLowerInvariant();
        return Array.IndexOf(AllowedMethodKeys, normalized) >= 0;
    }
}

