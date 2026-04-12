using CRM.Application.Features
using CRM.Application.Features.Shared;

namespace CRM.API.Tests.UseCases;

public sealed class PagingRequestTests
{
    [Fact]
    public void Normalize_ShouldClampLowerBounds()
    {
        var input = new PagingRequest(0, 0);

        var normalized = input.Normalize();

        Assert.Equal(PagingDefaults.DefaultPageNumber, normalized.PageNumber);
        Assert.Equal(1, normalized.PageSize);
    }

    [Fact]
    public void Normalize_ShouldClampUpperBounds()
    {
        var input = new PagingRequest(PagingDefaults.MaxPageNumber + 100, PagingDefaults.MaxPageSize + 100);

        var normalized = input.Normalize();

        Assert.Equal(PagingDefaults.MaxPageNumber, normalized.PageNumber);
        Assert.Equal(PagingDefaults.MaxPageSize, normalized.PageSize);
    }
}
