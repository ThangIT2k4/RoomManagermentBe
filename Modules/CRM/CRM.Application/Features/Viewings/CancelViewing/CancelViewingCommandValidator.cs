using CRM.Application.Validation;
using FluentValidation;

namespace CRM.Application.Features.Viewings.CancelViewing;

public sealed class CancelViewingCommandValidator : AbstractValidator<CancelViewingCommand>
{
    public CancelViewingCommandValidator()
    {
        RuleFor(x => x.ViewingId)
            .NotEmpty()
            .WithMessage("Mã lịch hẹn không được để trống.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Lý do hủy không được để trống.")
            .MaximumLength(1000)
            .WithMessage("Lý do hủy không được vượt quá 1000 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Lý do hủy chứa nội dung không an toàn.");

        RuleFor(x => x.RequestedBy)
            .NotEmpty()
            .WithMessage("Mã người yêu cầu không được để trống.");
    }
}
