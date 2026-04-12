using FluentValidation;

namespace Finance.Application.Features.Invoices.SearchInvoices;

public sealed class SearchInvoicesQueryValidator : AbstractValidator<SearchInvoicesQuery>
{
    public SearchInvoicesQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PerPage).InclusiveBetween(1, 200);
    }
}
