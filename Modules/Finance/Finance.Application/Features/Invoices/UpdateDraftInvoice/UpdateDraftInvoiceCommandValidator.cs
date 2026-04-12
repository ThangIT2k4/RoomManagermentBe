using FluentValidation;

namespace Finance.Application.Features.Invoices.UpdateDraftInvoice;

public sealed class UpdateDraftInvoiceCommandValidator : AbstractValidator<UpdateDraftInvoiceCommand>
{
    public UpdateDraftInvoiceCommandValidator()
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

        RuleFor(x => x.DueDate)
            .NotEmpty()
            .WithMessage("Ngày đến hạn không được để trống.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .WithMessage("Ghi chú không được vượt quá 2000 ký tự.");

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

            item.RuleFor(i => i.Description)
                .MaximumLength(2000)
                .WithMessage("Mô tả khoản thu không được vượt quá 2000 ký tự.")
                .When(i => !string.IsNullOrWhiteSpace(i.Description));

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .WithMessage("Số lượng phải lớn hơn 0.");

            item.RuleFor(i => i.UnitPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Đơn giá phải lớn hơn hoặc bằng 0.");
        });
    }
}
