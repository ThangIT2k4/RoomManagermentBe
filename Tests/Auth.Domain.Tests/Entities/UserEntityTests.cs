using Auth.Domain.Entities;
using Auth.Domain.Exceptions;

namespace Auth.Domain.Tests.Entities;

public sealed class UserEntityTests
{
    [Fact]
    public void Create_WithValidInput_ShouldTrimEmailAndUsername()
    {
        var entity = UserEntity.Create("  user@mail.com  ", "  demo  ");

        Assert.Equal("user@mail.com", entity.Email.Value);
        Assert.Equal("demo", entity.Username?.Value);
    }

    [Fact]
    public void Create_WithEmptyEmail_ShouldThrowDomainException()
    {
        Assert.Throws<InvalidEmailException>(() => UserEntity.Create(""));
    }
}

