using Auth.API.Validators;
using Auth.Application.Dtos;

namespace Auth.API.Tests.Validators;

public sealed class ResetPasswordRequestValidatorTests
{
    private readonly ResetPasswordRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenOtpIsNotNumeric()
    {
        var request = new ResetPasswordRequest("user@example.com", "12ab56", "Password1");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "OtpCode");
    }

    [Fact]
    public void Validate_ShouldPass_WhenOtpIsNumeric6Digits()
    {
        var request = new ResetPasswordRequest("user@example.com", "123456", "Password1");

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
