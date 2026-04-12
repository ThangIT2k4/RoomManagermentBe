using FluentValidation;

namespace CRM.Application.Features.Bookings.ApproveBooking;

public sealed class ApproveBookingCommandValidator : AbstractValidator<ApproveBookingCommand>
{
    public ApproveBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("Mã booking không được để trống.");

        RuleFor(x => x.ApprovedBy)
            .NotEmpty()
            .WithMessage("Mã người duyệt không được để trống.");
    }
}
