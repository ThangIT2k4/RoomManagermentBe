namespace CRM.Application.Features.Shared;

public static class PagingDefaults
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 200;
    public const int MaxPageNumber = 10_000;
}

public sealed record PagingRequest(
    int PageNumber = PagingDefaults.DefaultPageNumber,
    int PageSize = PagingDefaults.DefaultPageSize)
{
    public PagingRequest Normalize()
    {
        var safePageNumber = Math.Clamp(PageNumber, PagingDefaults.DefaultPageNumber, PagingDefaults.MaxPageNumber);
        var safePageSize = Math.Clamp(PageSize, 1, PagingDefaults.MaxPageSize);
        return new PagingRequest(safePageNumber, safePageSize);
    }
}

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    long TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);
