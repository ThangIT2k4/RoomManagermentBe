namespace Auth.Infrastructure.Options;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    /// <summary>SMTP host. If empty, OTP emails are not sent (OTP still stored for dev).</summary>
    public string? Host { get; set; }

    public int Port { get; set; } = 587;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public bool EnableSsl { get; set; } = true;

    public string? FromEmail { get; set; }

    public string? FromName { get; set; }
}
