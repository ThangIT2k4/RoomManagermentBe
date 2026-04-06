using Auth.API.Requests;
using Auth.API.Validators;

namespace Auth.API.Tests.Validators;

public sealed class UploadAvatarApiRequestValidatorTests
{
    private readonly UploadAvatarApiRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenAvatarUrlIsJavascriptScheme()
    {
        var request = new UploadAvatarApiRequest("javascript:alert(1)");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "AvatarUrl");
    }

    [Fact]
    public void Validate_ShouldPass_WhenAvatarUrlIsHttps()
    {
        var request = new UploadAvatarApiRequest("https://cdn.example.com/avatar.png");

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
