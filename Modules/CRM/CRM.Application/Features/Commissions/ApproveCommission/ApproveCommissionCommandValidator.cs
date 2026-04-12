using FluentValidation;

namespace CRM.Application.Features.Commissions.ApproveCommission;

public sealed class ApproveCommissionCommandValidator : AbstractValidator<ApproveCommissionCommand>
{
    public ApproveCommissionCommandValidator()
    {
        RuleFor(x => x.CommissionEventId)
            .NotEmpty()
            .WithMessage("Mã sự kiện hoa hồng không được để trống.");

        RuleFor(x => x.ApprovedBy)
            .NotEmpty()
            .WithMessage("Mã người duyệt không được để trống.");
    }
}
