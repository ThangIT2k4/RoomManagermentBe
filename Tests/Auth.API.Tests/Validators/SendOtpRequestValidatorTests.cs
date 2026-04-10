using Auth.API.Validators;
using Auth.Application.Dtos;

namespace Auth.API.Tests.Validators;

public sealed class SendOtpRequestValidatorTests
{
    private readonly SendOtpRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenPurposeIsOutOfEnum()
    {
        var request = new SendOtpRequest("user@example.com", (OtpPurpose)99, Guid.NewGuid());

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Purpose");
    }

    [Fact]
    public void Validate_ShouldPass_WhenPayloadIsValid()
    {
        var request = new SendOtpRequest("user@example.com", OtpPurpose.VerifyEmail, Guid.NewGuid());

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
