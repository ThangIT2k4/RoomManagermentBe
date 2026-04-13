using FluentValidation;

namespace Lease.Application.Features.Leases.SearchLeases;

public sealed class SearchLeasesQueryValidator : AbstractValidator<SearchLeasesQuery>
{
    private static readonly string[] AllowedStatuses = ["active", "terminated", "expired", "renewed"];

    public SearchLeasesQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.Statuses)
            .Must(IsAllowedStatuses)
            .When(x => !string.IsNullOrWhiteSpace(x.Statuses))
            .WithMessage("Danh sách trạng thái hợp đồng không hợp lệ.");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Số trang phải lớn hơn 0.");

        RuleFor(x => x.PerPage)
            .InclusiveBetween(1, 200).WithMessage("Số bản ghi mỗi trang phải từ 1 đến 200.");

        RuleFor(x => x.Search)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Search))
            .WithMessage("Từ khóa tìm kiếm không được vượt quá 200 ký tự.");
    }

    private static bool IsAllowedStatuses(string? statuses)
    {
        if (string.IsNullOrWhiteSpace(statuses))
        {
            return false;
        }

        var values = statuses.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return values.All(v => Array.IndexOf(AllowedStatuses, v.ToLowerInvariant()) >= 0);
    }
}
