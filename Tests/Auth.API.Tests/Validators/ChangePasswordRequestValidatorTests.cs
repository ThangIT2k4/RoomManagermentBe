using Auth.Application.Features.Auth.ChangePassword;
using Auth.Application.Validators;

namespace Auth.API.Tests.Validators;

public sealed class ChangePasswordRequestValidatorTests
{
    private readonly ChangePasswordCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenUserIdIsEmpty()
    {
        var command = new ChangePasswordCommand(Guid.Empty, "CurrentPass1", "NewPass123", null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "UserId");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNewPasswordEqualsCurrentPassword()
    {
        var command = new ChangePasswordCommand(Guid.NewGuid(), "SamePassword1", "SamePassword1", null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "NewPassword");
    }

    [Fact]
    public void Validate_ShouldPass_WhenPayloadIsValid()
    {
        var command = new ChangePasswordCommand(Guid.NewGuid(), "CurrentPass1", "NewPass123", null);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
