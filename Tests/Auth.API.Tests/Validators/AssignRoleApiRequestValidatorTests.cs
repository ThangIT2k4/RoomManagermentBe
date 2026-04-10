using Auth.API.Requests;
using Auth.API.Validators;

namespace Auth.API.Tests.Validators;

public sealed class AssignRoleApiRequestValidatorTests
{
    private readonly AssignRoleApiRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenOrganizationIdIsEmpty()
    {
        var request = new AssignRoleApiRequest(Guid.Empty, Guid.NewGuid());

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "OrganizationId");
    }

    [Fact]
    public void Validate_ShouldFail_WhenRoleIdIsEmpty()
    {
        var request = new AssignRoleApiRequest(Guid.NewGuid(), Guid.Empty);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "RoleId");
    }

    [Fact]
    public void Validate_ShouldPass_WhenPayloadIsValid()
    {
        var request = new AssignRoleApiRequest(Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
