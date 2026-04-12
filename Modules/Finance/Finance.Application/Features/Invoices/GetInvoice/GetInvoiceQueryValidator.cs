using FluentValidation;

namespace Finance.Application.Features.Invoices.GetInvoice;

public sealed class GetInvoiceQueryValidator : AbstractValidator<GetInvoiceQuery>
{
    public GetInvoiceQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.InvoiceId)
            .NotEmpty()
            .WithMessage("Mã hóa đơn không được để trống.");
    }
}

public sealed class GetInvoiceForTenantQueryValidator : AbstractValidator<GetInvoiceForTenantQuery>
{
    public GetInvoiceForTenantQueryValidator()
    {
        RuleFor(x => x.TenantUserId)
            .NotEmpty()
            .WithMessage("Mã người dùng tenant không được để trống.");

        RuleFor(x => x.InvoiceId)
            .NotEmpty()
            .WithMessage("Mã hóa đơn không được để trống.");
    }
}

