using Auth.API.Validators;
using Auth.Application.Dtos;

namespace Auth.API.Tests.Validators;

public sealed class VerifyOtpRequestValidatorTests
{
    private readonly VerifyOtpRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenOtpCodeIsNotSixDigits()
    {
        var request = new VerifyOtpRequest("user@example.com", OtpPurpose.VerifyEmail, "12345");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "OtpCode");
    }

    [Fact]
    public void Validate_ShouldPass_WhenOtpCodeIsValid()
    {
        var request = new VerifyOtpRequest("user@example.com", OtpPurpose.VerifyEmail, "123456");

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
