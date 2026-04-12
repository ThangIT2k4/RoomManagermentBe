using FluentValidation;

namespace Finance.Application.Features.Invoices.SearchInvoices;

public sealed class SearchInvoicesQueryValidator : AbstractValidator<SearchInvoicesQuery>
{
    private static readonly string[] AllowedStatuses = ["draft", "sent", "overdue", "paid", "cancelled"];

    public SearchInvoicesQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleForEach(x => x.Statuses)
            .Must(IsAllowedStatus)
            .WithMessage("Trạng thái hóa đơn không hợp lệ.")
            .When(x => x.Statuses is not null);

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate!.Value)
            .WithMessage("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.")
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue);

        RuleFor(x => x.Search)
            .MaximumLength(200)
            .WithMessage("Từ khóa tìm kiếm không được vượt quá 200 ký tự.")
            .When(x => !string.IsNullOrWhiteSpace(x.Search));

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Số trang phải lớn hơn 0.");

        RuleFor(x => x.PerPage)
            .InclusiveBetween(1, 200)
            .WithMessage("Số bản ghi mỗi trang phải từ 1 đến 200.");
    }

    private static bool IsAllowedStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return false;
        }

        var normalized = status.Trim().ToLowerInvariant();
        return Array.IndexOf(AllowedStatuses, normalized) >= 0;
    }
}

public sealed class ListTenantInvoicesQueryValidator : AbstractValidator<ListTenantInvoicesQuery>
{
    public ListTenantInvoicesQueryValidator()
    {
        RuleFor(x => x.TenantUserId)
            .NotEmpty()
            .WithMessage("Mã người dùng tenant không được để trống.");
    }
}
