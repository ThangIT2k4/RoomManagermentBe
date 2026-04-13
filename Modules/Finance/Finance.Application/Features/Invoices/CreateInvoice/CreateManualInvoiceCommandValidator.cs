using FluentValidation;

namespace Finance.Application.Features.Invoices.CreateInvoice;

public sealed class CreateManualInvoiceCommandValidator : AbstractValidator<CreateManualInvoiceCommand>
{
    private static readonly string[] AllowedItemTypes = ["rent", "service", "meter", "maintenance", "other"];

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

        RuleFor(x => x.InvoiceDate)
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30))
            .WithMessage("Ngày lập hóa đơn không được vượt quá 30 ngày so với hiện tại.");

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(x => x.InvoiceDate)
            .WithMessage("Ngày đến hạn phải lớn hơn hoặc bằng ngày lập hóa đơn.");

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
                .WithMessage("Loại mục không được vượt quá 100 ký tự.")
                .Must(IsAllowedItemType)
                .WithMessage("Loại mục không hợp lệ.");

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

    private static bool IsAllowedItemType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim().ToLowerInvariant();
        return Array.IndexOf(AllowedItemTypes, normalized) >= 0;
    }
}
