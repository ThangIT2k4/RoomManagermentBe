using Auth.Application.Features.Register;

namespace Auth.API.Tests.Validators;

public sealed class RegisterRequestValidatorTests
{
    private readonly RegisterValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenUsernameContainsScriptPayload()
    {
        var command = new RegisterCommand(
            "safe@example.com",
            "StrongPwd1",
            "Safe User",
            "<script>alert(1)</script>",
            "+84123456789");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Username");
    }

    [Fact]
    public void Validate_ShouldPass_WithSafePayload()
    {
        var command = new RegisterCommand(
            "safe@example.com",
            "StrongPwd1",
            "Safe User",
            "safe_user.01",
            "+84123456789");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
