using FluentValidation;

namespace Finance.Application.Features.Payments.SearchPayments;

public sealed class SearchPaymentsQueryValidator : AbstractValidator<SearchPaymentsQuery>
{
    private static readonly string[] AllowedStatuses = ["pending", "success", "failed", "cancelled"];

    public SearchPaymentsQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.FromPaidAtUtc)
            .LessThanOrEqualTo(x => x.ToPaidAtUtc!.Value)
            .WithMessage("Thời gian bắt đầu phải nhỏ hơn hoặc bằng thời gian kết thúc.")
            .When(x => x.FromPaidAtUtc.HasValue && x.ToPaidAtUtc.HasValue);

        RuleFor(x => x.Status)
            .MaximumLength(50)
            .WithMessage("Trạng thái thanh toán không được vượt quá 50 ký tự.")
            .Must(IsAllowedStatus)
            .WithMessage("Trạng thái thanh toán không hợp lệ.")
            .When(x => !string.IsNullOrWhiteSpace(x.Status));

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Số trang phải lớn hơn 0.");

        RuleFor(x => x.PerPage)
            .InclusiveBetween(1, 200)
            .WithMessage("Số bản ghi mỗi trang phải từ 1 đến 200.");
    }

    private static bool IsAllowedStatus(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim().ToLowerInvariant();
        return Array.IndexOf(AllowedStatuses, normalized) >= 0;
    }
}

public sealed class ListTenantPaymentsQueryValidator : AbstractValidator<ListTenantPaymentsQuery>
{
    public ListTenantPaymentsQueryValidator()
    {
        RuleFor(x => x.TenantUserId)
            .NotEmpty()
            .WithMessage("Mã người dùng tenant không được để trống.");
    }
}

