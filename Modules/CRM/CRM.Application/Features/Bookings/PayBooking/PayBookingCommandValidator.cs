using CRM.Application.Validation;
using FluentValidation;

namespace CRM.Application.Features.Bookings.PayBooking;

public sealed class PayBookingCommandValidator : AbstractValidator<PayBookingCommand>
{
    public PayBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("Mã đặt cọc không được để trống.");

        RuleFor(x => x.PaidAmount)
            .GreaterThan(0)
            .WithMessage("Số tiền thanh toán phải lớn hơn 0.");

        RuleFor(x => x.PaidAt)
            .NotEmpty()
            .WithMessage("Thời điểm thanh toán không được để trống.");

        RuleFor(x => x.PaymentMethod)
            .MaximumLength(100)
            .WithMessage("Phương thức thanh toán không được vượt quá 100 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Phương thức thanh toán chứa nội dung không an toàn.")
            .When(x => !string.IsNullOrWhiteSpace(x.PaymentMethod));

        RuleFor(x => x.RequestedBy)
            .NotEmpty()
            .WithMessage("Mã người yêu cầu không được để trống.");
    }
}
