using FluentValidation;

namespace Finance.Application.Features.Invoices.PublishInvoice;

public sealed class PublishInvoiceCommandValidator : AbstractValidator<PublishInvoiceCommand>
{
    public PublishInvoiceCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.InvoiceId)
            .NotEmpty()
            .WithMessage("Mã hóa đơn không được để trống.");
    }
}

