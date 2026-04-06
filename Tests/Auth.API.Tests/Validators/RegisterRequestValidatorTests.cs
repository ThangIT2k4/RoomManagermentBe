using Auth.API.Validators;
using Auth.Application.Dtos;

namespace Auth.API.Tests.Validators;

public sealed class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenUsernameContainsScriptPayload()
    {
        var request = new RegisterRequest(
            "safe@example.com",
            "StrongPwd1",
            "Safe User",
            "<script>alert(1)</script>",
            "+84123456789");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Username");
    }

    [Fact]
    public void Validate_ShouldPass_WithSafePayload()
    {
        var request = new RegisterRequest(
            "safe@example.com",
            "StrongPwd1",
            "Safe User",
            "safe_user.01",
            "+84123456789");

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
