using FluentValidation;

namespace Finance.Application.Features.DepositRefunds.CreateDepositRefund;

public sealed class CreateDepositRefundCommandValidator : AbstractValidator<CreateDepositRefundCommand>
{
    public CreateDepositRefundCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.LeaseId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
