using FluentValidation;

namespace Finance.Application.Features.Invoices.SearchInvoices;

public sealed class SearchInvoicesQueryValidator : AbstractValidator<SearchInvoicesQuery>
{
    public SearchInvoicesQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Số trang phải lớn hơn 0.");

        RuleFor(x => x.PerPage)
            .InclusiveBetween(1, 200)
            .WithMessage("Số bản ghi mỗi trang phải từ 1 đến 200.");
    }
}
