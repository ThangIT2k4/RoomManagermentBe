using FluentValidation;

namespace Finance.Application.Features.Invoices.CancelInvoice;

public sealed class CancelInvoiceCommandValidator : AbstractValidator<CancelInvoiceCommand>
{
    public CancelInvoiceCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Mã người dùng không được để trống.");

        RuleFor(x => x.InvoiceId)
            .NotEmpty()
            .WithMessage("Mã hóa đơn không được để trống.");

        RuleFor(x => x.Reason)
            .MaximumLength(2000)
            .WithMessage("Lý do không được vượt quá 2000 ký tự.")
            .When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}

