using Auth.API.Validators;
using Auth.Application.Dtos;

namespace Auth.API.Tests.Validators;

public sealed class ResendOtpRequestValidatorTests
{
    private readonly ResendOtpRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsInvalid()
    {
        var request = new ResendOtpRequest("not-an-email", OtpPurpose.VerifyEmail, Guid.NewGuid());

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Email");
    }

    [Fact]
    public void Validate_ShouldPass_WhenPayloadIsValid()
    {
        var request = new ResendOtpRequest("user@example.com", OtpPurpose.VerifyEmail, Guid.NewGuid());

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
