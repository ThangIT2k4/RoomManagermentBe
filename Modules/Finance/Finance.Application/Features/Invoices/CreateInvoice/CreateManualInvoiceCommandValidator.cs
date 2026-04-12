using FluentValidation;

namespace Finance.Application.Features.Invoices.CreateInvoice;

public sealed class CreateManualInvoiceCommandValidator : AbstractValidator<CreateManualInvoiceCommand>
{
    public CreateManualInvoiceCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Mã người dùng không được để trống.");

        RuleFor(x => x.LeaseId)
            .NotEmpty()
            .WithMessage("Mã hợp đồng thuê không được để trống.");

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(x => x.InvoiceDate)
            .WithMessage("Ngày đến hạn phải lớn hơn hoặc bằng ngày lập hóa đơn.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Hóa đơn phải có ít nhất một dòng chi tiết.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ItemType)
                .NotEmpty()
                .WithMessage("Loại mục không được để trống.")
                .MaximumLength(100)
                .WithMessage("Loại mục không được vượt quá 100 ký tự.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .WithMessage("Số lượng phải lớn hơn 0.");

            item.RuleFor(i => i.UnitPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Đơn giá phải lớn hơn hoặc bằng 0.");
        });
    }
}
