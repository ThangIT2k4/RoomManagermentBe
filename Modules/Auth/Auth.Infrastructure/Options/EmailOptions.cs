namespace Auth.Infrastructure.Options;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    /// <summary>SMTP host. If empty, OTP is not sent unless <see cref="AllowSendWithoutSmtp"/> is true.</summary>
    public string? Host { get; set; }

    /// <summary>
    /// When true and <see cref="Host"/> is empty, OTP flows still succeed (log cảnh báo). Dùng cho dev local.
    /// Production nên false để API báo lỗi cấu hình thay vì im lặng thành công.
    /// </summary>
    public bool AllowSendWithoutSmtp { get; set; }

    public int Port { get; set; } = 587;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public bool EnableSsl { get; set; } = true;

    public string? FromEmail { get; set; }

    public string? FromName { get; set; }
}
