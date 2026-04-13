using CRM.Application.Features.Leads.CreateLead;

namespace CRM.API.Tests.Validators;

public sealed class CreateLeadRequestValidatorTests
{
    private readonly CreateLeadCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenFullNameContainsScriptPayload()
    {
        var request = new CreateLeadRequest(Guid.NewGuid(), "<script>alert(1)</script>");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "FullName");
    }

    [Fact]
    public void Validate_ShouldFail_WhenStatusLooksLikeSqlInjection()
    {
        var request = new CreateLeadRequest(Guid.NewGuid(), "Alice", "new; DROP TABLE leads;");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Status");
    }

    [Fact]
    public void Validate_ShouldPass_WithSafePayload()
    {
        var request = new CreateLeadRequest(Guid.NewGuid(), "Alice Nguyen", "qualified");

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
