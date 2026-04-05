namespace Auth.Domain.Common;

public readonly record struct PagingInput(int PageNumber, int PageSize, int Skip)
{
    public static PagingInput Create(int pageNumber, int pageSize)
    {
        var normalizedPageNumber = pageNumber <= 0
            ? InputSecurityLimits.DefaultPageNumber
            : Math.Min(pageNumber, InputSecurityLimits.MaxPageNumber);

        var requestedPageSize = pageSize <= 0
            ? InputSecurityLimits.DefaultPageSize
            : pageSize;

        var normalizedPageSize = Math.Min(requestedPageSize, InputSecurityLimits.MaxPageSize);

        var skipLong = (long)(normalizedPageNumber - 1) * normalizedPageSize;
        var normalizedSkip = skipLong > int.MaxValue ? int.MaxValue : (int)skipLong;

        return new PagingInput(normalizedPageNumber, normalizedPageSize, normalizedSkip);
    }
}

