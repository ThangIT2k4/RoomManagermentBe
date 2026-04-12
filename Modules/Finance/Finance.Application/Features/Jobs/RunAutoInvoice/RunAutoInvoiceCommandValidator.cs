using FluentValidation;

namespace Finance.Application.Features.Jobs.RunAutoInvoice;

public sealed class RunAutoInvoiceCommandValidator : AbstractValidator<RunAutoInvoiceCommand>
{
    public RunAutoInvoiceCommandValidator()
    {
        RuleFor(x => x.RunDate)
            .NotEmpty()
            .WithMessage("Ngày chạy tạo hóa đơn không được để trống.");
    }
}

