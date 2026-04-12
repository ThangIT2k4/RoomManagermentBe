using Auth.Application.Features.Auth.VerifyEmail;
using Auth.Application.Validators;

namespace Auth.API.Tests.Validators;

public sealed class VerifyEmailRequestValidatorTests
{
    private readonly VerifyEmailCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenOtpCodeContainsLetters()
    {
        var command = new VerifyEmailCommand("user@example.com", "12AB56");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "OtpCode");
    }

    [Fact]
    public void Validate_ShouldPass_WhenPayloadIsValid()
    {
        var command = new VerifyEmailCommand("user@example.com", "123456");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
