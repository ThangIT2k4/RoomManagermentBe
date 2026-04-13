using Auth.Application.Features.Users.UpdateUser;

namespace Auth.API.Tests.Validators;

public sealed class UpdateUserCommandValidatorTests
{
    private readonly UpdateUserCommandValidator _validator = new();
    private static readonly Guid UserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public void Validate_ShouldFail_WhenNoFieldIsProvided()
    {
        var command = new UpdateUserCommand(UserId, null, null, null, null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.ErrorMessage.Contains("một trường để cập nhật"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenUsernameHasInvalidCharacter()
    {
        var command = new UpdateUserCommand(UserId, null, "john doe", null, null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Username");
    }

    [Fact]
    public void Validate_ShouldPass_WhenStatusOnlyIsProvided()
    {
        var command = new UpdateUserCommand(UserId, null, null, null, 2);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
