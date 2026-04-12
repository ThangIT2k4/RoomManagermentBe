using CRM.Application.Validation;
using FluentValidation;

namespace CRM.Application.Features.Viewings.CreateViewing;

public sealed class CreateViewingCommandValidator : AbstractValidator<CreateViewingCommand>
{
    public CreateViewingCommandValidator()
    {
        RuleFor(x => x.LeadId)
            .NotEmpty()
            .WithMessage("Mã khách hàng tiềm năng không được để trống.");

        RuleFor(x => x.ScheduledAt)
            .Must(date => date > DateTime.UtcNow.AddMinutes(30))
            .WithMessage("Thời gian xem phòng phải sau hiện tại ít nhất 30 phút.");

        RuleFor(x => x.Location)
            .MaximumLength(255)
            .WithMessage("Địa điểm không được vượt quá 255 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Địa điểm chứa nội dung không an toàn.")
            .When(x => !string.IsNullOrWhiteSpace(x.Location));

        RuleFor(x => x.AgentUserId)
            .NotEmpty()
            .WithMessage("Mã nhân viên dẫn xem không được để trống.");

        RuleFor(x => x.Note)
            .MaximumLength(1000)
            .WithMessage("Ghi chú không được vượt quá 1000 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Ghi chú chứa nội dung không an toàn.")
            .When(x => !string.IsNullOrWhiteSpace(x.Note));
    }
}
