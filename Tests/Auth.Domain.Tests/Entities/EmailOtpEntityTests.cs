using Auth.Domain.Entities;
using Auth.Domain.Enums;
using Auth.Domain.Exceptions;

namespace Auth.Domain.Tests.Entities;

public sealed class EmailOtpEntityTests
{
    [Fact]
    public void CanResend_ShouldRespectThrottleWindow()
    {
        var createdAt = DateTime.UtcNow;
        var otp = EmailOtpEntity.Issue(
            Guid.NewGuid(),
            "user@example.com",
            "123456",
            EmailOtpType.VerifyEmail,
            DateTimeOffset.UtcNow.AddMinutes(5),
            createdAt);

        var beforeWindow = otp.CanResend(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1));
        var afterWindow = otp.CanResend(new DateTimeOffset(createdAt.AddMinutes(2)), TimeSpan.FromMinutes(1));

        Assert.False(beforeWindow);
        Assert.True(afterWindow);
    }

    [Fact]
    public void MarkVerified_ShouldSetUsedAndVerifiedAt()
    {
        var otp = EmailOtpEntity.Issue(
            Guid.NewGuid(),
            "user@example.com",
            "123456",
            EmailOtpType.VerifyEmail,
            DateTimeOffset.UtcNow.AddMinutes(5));
        var verifiedAt = DateTimeOffset.UtcNow;

        otp.MarkVerified(verifiedAt);

        Assert.True(otp.IsUsed);
        Assert.Equal(verifiedAt, otp.VerifiedAt);
    }

    [Fact]
    public void MarkVerified_ShouldThrow_WhenOtpExpired()
    {
        var otp = EmailOtpEntity.Issue(
            Guid.NewGuid(),
            "user@example.com",
            "123456",
            EmailOtpType.VerifyEmail,
            DateTimeOffset.UtcNow.AddSeconds(-1));

        var ex = Assert.Throws<InvalidEmailOtpException>(() => otp.MarkVerified(DateTimeOffset.UtcNow));

        Assert.Equal(InvalidEmailOtpException.CodeExpired, ex.ErrorCode);
    }

    [Fact]
    public void MarkUsed_Twice_ShouldThrowUsedException()
    {
        var otp = EmailOtpEntity.Issue(
            Guid.NewGuid(),
            "user@example.com",
            "123456",
            EmailOtpType.ResetPassword,
            DateTimeOffset.UtcNow.AddMinutes(5));

        otp.MarkUsed(DateTimeOffset.UtcNow);
        var ex = Assert.Throws<InvalidEmailOtpException>(() => otp.MarkUsed(DateTimeOffset.UtcNow));

        Assert.Equal(InvalidEmailOtpException.CodeUsed, ex.ErrorCode);
    }
}
