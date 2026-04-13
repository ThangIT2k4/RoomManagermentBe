using Auth.Application.Features.Auth.ResetPassword;
namespace Auth.API.Tests.Validators;

public sealed class ResetPasswordRequestValidatorTests
{
    private readonly ResetPasswordCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenOtpIsNotNumeric()
    {
        var command = new ResetPasswordCommand("user@example.com", "12ab56", "Password1");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "OtpCode");
    }

    [Fact]
    public void Validate_ShouldPass_WhenOtpIsNumeric6Digits()
    {
        var command = new ResetPasswordCommand("user@example.com", "123456", "Password1");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
