using Auth.Domain.Common;

namespace Auth.Domain.Tests.Common;

public sealed class QueryInputTests
{
    [Fact]
    public void SearchInputNormalize_ShouldReturnNull_WhenWhitespace()
    {
        var normalized = SearchInput.Normalize("   ");

        Assert.Null(normalized);
    }

    [Fact]
    public void SearchInputNormalize_ShouldTrimValue()
    {
        var normalized = SearchInput.Normalize("  john  ");

        Assert.Equal("john", normalized);
    }

    [Fact]
    public void PagingInputCreate_ShouldClampAndCalculateSkip()
    {
        var paging = PagingInput.Create(0, 999);

        Assert.Equal(1, paging.PageNumber);
        Assert.Equal(500, paging.PageSize);
        Assert.Equal(0, paging.Skip);
    }

    [Fact]
    public void PagingInputCreate_ShouldComputeSkipForLaterPage()
    {
        var paging = PagingInput.Create(3, 25);

        Assert.Equal(50, paging.Skip);
    }
}
