using Auth.Infrastructure.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Auth.Infrastructure.Mail;

public sealed class SmtpEmailSender(IOptions<EmailOptions> options) : IEmailSender
{
    private readonly EmailOptions _options = options.Value;

    public async Task SendAsync(string toEmail, string subject, string textBody, string? htmlBody, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Host))
        {
            throw new InvalidOperationException("Email Host is not configured.");
        }

        var fromEmail = _options.FromEmail?.Trim();
        if (string.IsNullOrWhiteSpace(fromEmail))
        {
            throw new InvalidOperationException("Email FromEmail is required when Host is set.");
        }

        var fromName = string.IsNullOrWhiteSpace(_options.FromName) ? fromEmail : _options.FromName.Trim();

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail.Trim()));
        message.Subject = subject;

        var plain = new TextPart("plain") { Text = textBody };
        if (!string.IsNullOrWhiteSpace(htmlBody))
        {
            var html = new TextPart("html") { Text = htmlBody };
            message.Body = new MultipartAlternative(plain, html);
        }
        else
        {
            message.Body = plain;
        }

        using var client = new SmtpClient();
        var secure = _options.EnableSsl
            ? _options.Port == 465
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls
            : SecureSocketOptions.None;

        await client.ConnectAsync(_options.Host.Trim(), _options.Port, secure, cancellationToken);

        var user = _options.Username?.Trim();
        if (!string.IsNullOrEmpty(user))
        {
            await client.AuthenticateAsync(user, _options.Password ?? string.Empty, cancellationToken);
        }

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
