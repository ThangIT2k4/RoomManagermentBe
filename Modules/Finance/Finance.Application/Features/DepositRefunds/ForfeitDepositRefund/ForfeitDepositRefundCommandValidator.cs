using FluentValidation;

namespace Finance.Application.Features.DepositRefunds.ForfeitDepositRefund;

public sealed class ForfeitDepositRefundCommandValidator : AbstractValidator<ForfeitDepositRefundCommand>
{
    public ForfeitDepositRefundCommandValidator()
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

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Lý do không được để trống.")
            .MaximumLength(2000)
            .WithMessage("Lý do không được vượt quá 2000 ký tự.");
    }
}

