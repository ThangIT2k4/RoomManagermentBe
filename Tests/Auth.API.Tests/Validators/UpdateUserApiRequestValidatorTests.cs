using Auth.API.Requests;
using Auth.API.Validators;

namespace Auth.API.Tests.Validators;

public sealed class UpdateUserApiRequestValidatorTests
{
    private readonly UpdateUserApiRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenNoFieldIsProvided()
    {
        var request = new UpdateUserApiRequest(null, null, null, null);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.ErrorMessage.Contains("At least one field must be provided"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenUsernameHasInvalidCharacter()
    {
        var request = new UpdateUserApiRequest(null, "john doe", null, null);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Username");
    }

    [Fact]
    public void Validate_ShouldPass_WhenStatusOnlyIsProvided()
    {
        var request = new UpdateUserApiRequest(null, null, null, 2);

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
