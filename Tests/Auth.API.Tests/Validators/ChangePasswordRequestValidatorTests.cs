using Auth.API.Validators;
using Auth.Application.Dtos;

namespace Auth.API.Tests.Validators;

public sealed class ChangePasswordRequestValidatorTests
{
    private readonly ChangePasswordRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenUserIdIsEmpty()
    {
        var request = new ChangePasswordRequest(Guid.Empty, "CurrentPass1", "NewPass123", null);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "UserId");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNewPasswordEqualsCurrentPassword()
    {
        var request = new ChangePasswordRequest(Guid.NewGuid(), "SamePassword1", "SamePassword1", null);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "NewPassword");
    }

    [Fact]
    public void Validate_ShouldPass_WhenPayloadIsValid()
    {
        var request = new ChangePasswordRequest(Guid.NewGuid(), "CurrentPass1", "NewPass123", null);

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
