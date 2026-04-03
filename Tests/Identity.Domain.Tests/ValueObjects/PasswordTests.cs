using Identity.Domain.Exceptions;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Tests.ValueObjects;

public sealed class PasswordTests
{
    [Fact]
    public void Create_WithValidPassword_ReturnsPassword()
    {
        var password = Password.Create("Str0ng@Pass");

        Assert.Equal("Str0ng@Pass", password.Value);
    }

    [Fact]
    public void Create_WithTooShortPassword_ThrowsInvalidPasswordException()
    {
        var ex = Assert.Throws<InvalidPasswordException>(() => Password.Create("Aa1@a"));

        Assert.Equal(InvalidPasswordException.CodeTooShort, ex.ErrorCode);
    }

    [Fact]
    public void Create_WithWhitespace_ThrowsInvalidPasswordException()
    {
        var ex = Assert.Throws<InvalidPasswordException>(() => Password.Create("Abc 123@"));

        Assert.Equal(InvalidPasswordException.CodeNoWhitespace, ex.ErrorCode);
    }
}

