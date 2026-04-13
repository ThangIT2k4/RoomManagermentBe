using Auth.Application.Dtos;
using Auth.Application.Features.Auth.VerifyOtp;
namespace Auth.API.Tests.Validators;

public sealed class VerifyOtpRequestValidatorTests
{
    private readonly VerifyOtpCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenOtpCodeIsNotSixDigits()
    {
        var command = new VerifyOtpCommand("user@example.com", OtpPurpose.VerifyEmail, "12345");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "OtpCode");
    }

    [Fact]
    public void Validate_ShouldPass_WhenOtpCodeIsValid()
    {
        var command = new VerifyOtpCommand("user@example.com", OtpPurpose.VerifyEmail, "123456");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
