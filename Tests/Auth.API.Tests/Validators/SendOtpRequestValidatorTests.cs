using Auth.Application.Dtos;
using Auth.Application.Features.Auth.SendOtp;

namespace Auth.API.Tests.Validators;

public sealed class SendOtpRequestValidatorTests
{
    private readonly SendOtpCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenPurposeIsOutOfEnum()
    {
        var command = new SendOtpCommand("user@example.com", (OtpPurpose)99, Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Purpose");
    }

    [Fact]
    public void Validate_ShouldPass_WhenPayloadIsValid()
    {
        var command = new SendOtpCommand("user@example.com", OtpPurpose.VerifyEmail, Guid.NewGuid());

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
