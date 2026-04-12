using FluentValidation;

namespace Finance.Application.Features.DepositRefunds.CreateDepositRefund;

public sealed class CreateDepositRefundCommandValidator : AbstractValidator<CreateDepositRefundCommand>
{
    public CreateDepositRefundCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Mã người dùng không được để trống.");

        RuleFor(x => x.LeaseId)
            .NotEmpty()
            .WithMessage("Mã hợp đồng thuê không được để trống.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Số tiền phải lớn hơn 0.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .WithMessage("Ghi chú không được vượt quá 2000 ký tự.");
    }
}
