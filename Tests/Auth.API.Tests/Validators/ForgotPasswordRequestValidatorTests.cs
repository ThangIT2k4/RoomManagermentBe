using Auth.API.Validators;
using Auth.Application.Dtos;

namespace Auth.API.Tests.Validators;

public sealed class ForgotPasswordRequestValidatorTests
{
    private readonly ForgotPasswordRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenEmailContainsScriptPayload()
    {
        var request = new ForgotPasswordRequest("<script>alert(1)</script>@example.com");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Email");
    }

    [Fact]
    public void Validate_ShouldPass_WhenEmailIsNormal()
    {
        var request = new ForgotPasswordRequest("normal.user@example.com");

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
