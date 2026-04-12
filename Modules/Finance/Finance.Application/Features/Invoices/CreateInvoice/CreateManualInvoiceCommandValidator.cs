using FluentValidation;

namespace Finance.Application.Features.Invoices.CreateInvoice;

public sealed class CreateManualInvoiceCommandValidator : AbstractValidator<CreateManualInvoiceCommand>
{
    public CreateManualInvoiceCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.LeaseId).NotEmpty();
        RuleFor(x => x.DueDate).GreaterThanOrEqualTo(x => x.InvoiceDate);
        RuleFor(x => x.Items).NotEmpty().WithMessage("Invoice must have at least one line item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ItemType).NotEmpty().MaximumLength(100);
            item.RuleFor(i => i.Quantity).GreaterThan(0);
            item.RuleFor(i => i.UnitPrice).GreaterThanOrEqualTo(0);
        });
    }
}
