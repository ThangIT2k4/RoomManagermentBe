using CRM.Application.Validation;
using FluentValidation;

namespace CRM.Application.Features.Viewings.CompleteViewing;

public sealed class CompleteViewingCommandValidator : AbstractValidator<CompleteViewingCommand>
{
    public CompleteViewingCommandValidator()
    {
        RuleFor(x => x.ViewingId)
            .NotEmpty()
            .WithMessage("Mã lịch hẹn không được để trống.");

        RuleFor(x => x.Summary)
            .MaximumLength(2000)
            .WithMessage("Tóm tắt buổi xem không được vượt quá 2000 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Tóm tắt buổi xem chứa nội dung không an toàn.")
            .When(x => !string.IsNullOrWhiteSpace(x.Summary));
    }
}
