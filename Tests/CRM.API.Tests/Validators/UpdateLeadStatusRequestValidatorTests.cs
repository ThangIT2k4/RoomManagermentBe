using CRM.API.Validators;
using CRM.Application.Features.Leads;
using CRM.Application.Features.Leads.UpdateLeadStatus;

namespace CRM.API.Tests.Validators;

public sealed class UpdateLeadStatusRequestValidatorTests
{
    private readonly UpdateLeadStatusRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenLeadIdIsEmpty()
    {
        var request = new UpdateLeadStatusRequest(Guid.Empty, "qualified");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "LeadId");
    }

    [Fact]
    public void Validate_ShouldFail_WhenStatusContainsScriptPayload()
    {
        var request = new UpdateLeadStatusRequest(Guid.NewGuid(), "<script>alert(1)</script>");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Status");
    }

    [Fact]
    public void Validate_ShouldPass_WithSafeStatus()
    {
        var request = new UpdateLeadStatusRequest(Guid.NewGuid(), "contacted");

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
