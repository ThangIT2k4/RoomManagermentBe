using FluentValidation;

namespace Finance.Application.Features.Reports.GetDebtSummary;

public sealed class GetDebtSummaryQueryValidator : AbstractValidator<GetDebtSummaryQuery>
{
    public GetDebtSummaryQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");
    }
}

