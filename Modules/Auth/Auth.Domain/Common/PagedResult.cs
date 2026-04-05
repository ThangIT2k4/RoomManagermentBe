namespace Auth.Domain.Common;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public long TotalCount { get; }
    public int TotalPages { get; }

    private PagedResult(IReadOnlyList<T> items, int pageNumber, int pageSize, long totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public static PagedResult<T> Create(IReadOnlyList<T> items, int pageNumber, int pageSize, long totalCount)
    {
        var paging = PagingInput.Create(pageNumber, pageSize);
        var normalizedTotalCount = totalCount < 0 ? 0 : totalCount;
        return new PagedResult<T>(items, paging.PageNumber, paging.PageSize, normalizedTotalCount);
    }
}
