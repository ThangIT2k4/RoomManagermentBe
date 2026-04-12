using FluentValidation;

namespace CRM.Application.Features.Viewings.ConfirmViewing;

public sealed class ConfirmViewingCommandValidator : AbstractValidator<ConfirmViewingCommand>
{
    public ConfirmViewingCommandValidator()
    {
        RuleFor(x => x.ViewingId)
            .NotEmpty()
            .WithMessage("Mã lịch hẹn không được để trống.");
    }
}
