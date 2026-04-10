using Auth.Domain.Entities;
using Auth.Domain.Exceptions;

namespace Auth.Domain.Tests.Entities;

public sealed class SessionEntityTests
{
    private const string ValidToken = "0123456789abcdef";

    [Fact]
    public void CreateSessionCookie_ShouldNotSetExpiry()
    {
        var session = SessionEntity.CreateSessionCookie(Guid.NewGuid(), ValidToken, " 127.0.0.1 ", " Chrome ");

        Assert.Null(session.ExpiresAt);
        Assert.Equal("127.0.0.1", session.IpAddress);
        Assert.Equal("Chrome", session.UserAgent);
        Assert.False(session.IsExpired(DateTimeOffset.UtcNow));
    }

    [Fact]
    public void CreateRemembered_WithNonPositiveTtl_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            SessionEntity.CreateRemembered(Guid.NewGuid(), ValidToken, TimeSpan.Zero));
    }

    [Fact]
    public void Refresh_ShouldThrow_WhenSessionExpired()
    {
        var now = DateTimeOffset.UtcNow;
        var session = SessionEntity.Create(Guid.NewGuid(), ValidToken, expiresAt: now.AddMinutes(-1));

        var ex = Assert.Throws<InvalidSessionException>(() => session.Refresh(now));

        Assert.Equal(InvalidSessionException.CodeExpired, ex.ErrorCode);
    }

    [Fact]
    public void Revoke_ShouldMarkSessionAsExpired()
    {
        var session = SessionEntity.Create(Guid.NewGuid(), ValidToken);
        var now = DateTimeOffset.UtcNow;

        session.Revoke(now);

        Assert.True(session.IsRevoked);
        Assert.True(session.IsExpired(now));
    }
}
