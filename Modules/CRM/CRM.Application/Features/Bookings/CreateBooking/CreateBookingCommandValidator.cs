using CRM.Application.Validation;
using FluentValidation;

namespace CRM.Application.Features.Bookings.CreateBooking;

public sealed class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.LeadId)
            .NotEmpty()
            .WithMessage("Mã khách hàng tiềm năng không được để trống.");

        RuleFor(x => x.DepositAmount)
            .GreaterThan(0)
            .WithMessage("Số tiền đặt cọc phải lớn hơn 0.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Đơn vị tiền tệ không được để trống.")
            .MaximumLength(10)
            .WithMessage("Đơn vị tiền tệ không được vượt quá 10 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Đơn vị tiền tệ chứa nội dung không an toàn.");

        RuleFor(x => x.ExpiredAt)
            .Must(date => date > DateTime.UtcNow)
            .WithMessage("Thời điểm hết hạn phải lớn hơn thời điểm hiện tại.");

        RuleFor(x => x.RequestedBy)
            .NotEmpty()
            .WithMessage("Mã người yêu cầu không được để trống.");
    }
}
