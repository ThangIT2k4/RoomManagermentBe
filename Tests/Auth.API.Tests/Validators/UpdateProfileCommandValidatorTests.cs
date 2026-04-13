using Auth.Application.Features.Auth.Profile.UpdateProfile;

namespace Auth.API.Tests.Validators;

public sealed class UpdateProfileCommandValidatorTests
{
    private readonly UpdateProfileCommandValidator _validator = new();
    private static readonly Guid UserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public void Validate_ShouldFail_WhenNoteContainsScriptPayload()
    {
        var command = new UpdateProfileCommand(UserId, "John", null, null, "HCM", "<script>alert(1)</script>");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Note");
    }

    [Fact]
    public void Validate_ShouldPass_WhenProfilePayloadIsSafe()
    {
        var command = new UpdateProfileCommand(UserId, "John Doe", null, 1, "HCM City", "Safe note content");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
