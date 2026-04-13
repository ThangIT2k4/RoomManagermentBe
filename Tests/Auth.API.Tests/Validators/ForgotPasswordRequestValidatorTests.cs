using Auth.Application.Features.Auth.ForgotPassword;
namespace Auth.API.Tests.Validators;

public sealed class ForgotPasswordRequestValidatorTests
{
    private readonly ForgotPasswordCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenEmailContainsScriptPayload()
    {
        var command = new ForgotPasswordCommand("<script>alert(1)</script>@example.com");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Email");
    }

    [Fact]
    public void Validate_ShouldPass_WhenEmailIsNormal()
    {
        var command = new ForgotPasswordCommand("normal.user@example.com");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
