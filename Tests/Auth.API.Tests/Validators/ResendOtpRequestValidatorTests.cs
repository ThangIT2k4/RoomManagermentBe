using Auth.Application.Dtos;
using Auth.Application.Features.Auth.ResendOtp;
using Auth.Application.Validators;

namespace Auth.API.Tests.Validators;

public sealed class ResendOtpRequestValidatorTests
{
    private readonly ResendOtpCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsInvalid()
    {
        var command = new ResendOtpCommand("not-an-email", OtpPurpose.VerifyEmail, Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Email");
    }

    [Fact]
    public void Validate_ShouldPass_WhenPayloadIsValid()
    {
        var command = new ResendOtpCommand("user@example.com", OtpPurpose.VerifyEmail, Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
