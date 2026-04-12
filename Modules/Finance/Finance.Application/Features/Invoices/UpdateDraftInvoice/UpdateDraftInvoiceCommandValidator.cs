using FluentValidation;

namespace Finance.Application.Features.Invoices.UpdateDraftInvoice;

public sealed class UpdateDraftInvoiceCommandValidator : AbstractValidator<UpdateDraftInvoiceCommand>
{
    public UpdateDraftInvoiceCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty().WithMessage("Invoice must have at least one line item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ItemType).NotEmpty().MaximumLength(100);
            item.RuleFor(i => i.Quantity).GreaterThan(0);
            item.RuleFor(i => i.UnitPrice).GreaterThanOrEqualTo(0);
        });
    }
}
