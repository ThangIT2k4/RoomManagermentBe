using Auth.Application.Features.Users.AssignRole;
// using Auth.Application.Validators;
namespace Auth.API.Tests.Validators;

public sealed class AssignRoleCommandValidatorTests
{
    private readonly AssignRoleCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenOrganizationIdIsEmpty()
    {
        var command = new AssignRoleCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "OrganizationId");
    }

    [Fact]
    public void Validate_ShouldFail_WhenRoleIdIsEmpty()
    {
        var command = new AssignRoleCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "RoleId");
    }

    [Fact]
    public void Validate_ShouldPass_WhenPayloadIsValid()
    {
        var command = new AssignRoleCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
