using Auth.API.Validators;
using Auth.Application.Dtos;

namespace Auth.API.Tests.Validators;

public sealed class VerifyEmailRequestValidatorTests
{
    private readonly VerifyEmailRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenOtpCodeContainsLetters()
    {
        var request = new VerifyEmailRequest("user@example.com", "12AB56");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "OtpCode");
    }

    [Fact]
    public void Validate_ShouldPass_WhenPayloadIsValid()
    {
        var request = new VerifyEmailRequest("user@example.com", "123456");

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
