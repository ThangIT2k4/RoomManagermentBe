using FluentValidation;

namespace Finance.Application.Features.Reports.GetRevenueByMonth;

public sealed class GetRevenueByMonthQueryValidator : AbstractValidator<GetRevenueByMonthQuery>
{
    public GetRevenueByMonthQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100)
            .WithMessage("Năm báo cáo phải từ 2000 đến 2100.");
    }
}

