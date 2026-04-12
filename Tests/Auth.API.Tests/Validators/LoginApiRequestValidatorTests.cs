using Auth.Application.Features.Auth.Login;
using Auth.Application.Validators;

namespace Auth.API.Tests.Validators;

public sealed class LoginApiRequestValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenLoginLooksLikeSqlInjection()
    {
        var command = new LoginCommand("admin' OR 1=1 --", "Password1", null, null, false);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Login");
    }

    [Fact]
    public void Validate_ShouldPass_WhenLoginIsNormalEmail()
    {
        var command = new LoginCommand("normal.user@example.com", "Password1", null, null, false);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
