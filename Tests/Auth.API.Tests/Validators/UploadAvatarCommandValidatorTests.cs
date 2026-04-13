using Auth.Application.Features.Auth.Profile.UploadAvatar;

namespace Auth.API.Tests.Validators;

public sealed class UploadAvatarCommandValidatorTests
{
    private readonly UploadAvatarCommandValidator _validator = new();
    private static readonly Guid UserId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    [Fact]
    public void Validate_ShouldFail_WhenAvatarUrlIsJavascriptScheme()
    {
        var command = new UploadAvatarCommand(UserId, "javascript:alert(1)");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "AvatarUrl");
    }

    [Fact]
    public void Validate_ShouldPass_WhenAvatarUrlIsHttps()
    {
        var command = new UploadAvatarCommand(UserId, "https://cdn.example.com/avatar.png");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
