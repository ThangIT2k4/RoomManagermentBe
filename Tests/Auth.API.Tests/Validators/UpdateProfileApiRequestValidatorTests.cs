using Auth.API.Requests;
using Auth.API.Validators;

namespace Auth.API.Tests.Validators;

public sealed class UpdateProfileApiRequestValidatorTests
{
    private readonly UpdateProfileApiRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenNoteContainsScriptPayload()
    {
        var request = new UpdateProfileApiRequest("John", null, null, "HCM", "<script>alert(1)</script>");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Note");
    }

    [Fact]
    public void Validate_ShouldPass_WhenProfilePayloadIsSafe()
    {
        var request = new UpdateProfileApiRequest("John Doe", null, 1, "HCM City", "Safe note content");

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
