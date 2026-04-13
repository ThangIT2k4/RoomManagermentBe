namespace Auth.Infrastructure.Options;

public sealed class OtpOptions
{
    public const string SectionName = "Otp";

    /// <summary>Minimum wait between OTP send operations per email and purpose.</summary>
    public int ResendCooldownMinutes { get; set; } = 2;

    public int VerifyEmailExpiryMinutes { get; set; } = 5;

    public int ResetPasswordExpiryMinutes { get; set; } = 15;
}
