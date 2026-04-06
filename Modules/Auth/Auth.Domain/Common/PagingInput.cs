namespace Auth.Domain.Common;

public readonly record struct PagingInput(int PageNumber, int PageSize)
{
    public static PagingInput Create(int pageNumber, int pageSize)
    {
        var p = Math.Max(1, pageNumber);
        var s = Math.Clamp(pageSize, 1, 500);
        return new PagingInput(p, s);
    }

    public int Skip => (PageNumber - 1) * PageSize;
}
