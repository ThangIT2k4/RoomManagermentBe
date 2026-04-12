using FluentValidation;

namespace Finance.Application.Features.Payments.RecordPayment;

public sealed class RecordManualPaymentCommandValidator : AbstractValidator<RecordManualPaymentCommand>
{
    public RecordManualPaymentCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Mã người dùng không được để trống.");

        RuleFor(x => x.InvoiceId)
            .NotEmpty()
            .WithMessage("Mã hóa đơn không được để trống.");

        RuleFor(x => x.MethodId)
            .NotEmpty()
            .WithMessage("Mã phương thức thanh toán không được để trống.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Số tiền phải lớn hơn 0.");

        RuleFor(x => x.PaidAtUtc)
            .NotEmpty()
            .WithMessage("Thời điểm thanh toán không được để trống.");
    }
}
