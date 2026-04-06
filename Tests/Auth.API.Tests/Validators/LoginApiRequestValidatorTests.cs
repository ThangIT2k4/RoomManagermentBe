using Auth.API.Requests;
using Auth.API.Validators;

namespace Auth.API.Tests.Validators;

public sealed class LoginApiRequestValidatorTests
{
    private readonly LoginApiRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenLoginLooksLikeSqlInjection()
    {
        var request = new LoginApiRequest("admin' OR 1=1 --", "Password1", false);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Login");
    }

    [Fact]
    public void Validate_ShouldPass_WhenLoginIsNormalEmail()
    {
        var request = new LoginApiRequest("normal.user@example.com", "Password1", false);

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
