using Auth.Infrastructure.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Auth.Infrastructure.Mail;

public sealed class SmtpEmailSender(IOptionsMonitor<EmailOptions> optionsMonitor, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    public async Task SendAsync(string toEmail, string subject, string textBody, string? htmlBody, CancellationToken cancellationToken = default)
    {
        var opts = optionsMonitor.CurrentValue;

        if (string.IsNullOrWhiteSpace(opts.Host))
        {
            throw new InvalidOperationException(
                "SMTP host is not configured (Email:Host empty). Caller should check options before invoking SendAsync.");
        }

        var fromEmail = opts.FromEmail?.Trim();
        if (string.IsNullOrWhiteSpace(fromEmail) && !string.IsNullOrWhiteSpace(opts.Username) && opts.Username.Contains('@', StringComparison.Ordinal))
        {
            fromEmail = opts.Username.Trim();
        }

        if (string.IsNullOrWhiteSpace(fromEmail))
        {
            throw new InvalidOperationException(
                "Email FromEmail is not configured. Set Email:FromEmail or EMAIL_FROM_EMAIL (or use EMAIL_USERNAME as a full email address).");
        }

        var fromName = string.IsNullOrWhiteSpace(opts.FromName) ? fromEmail : opts.FromName.Trim();

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

        var host = opts.Host.Trim();
        var secure = opts.EnableSsl
            ? opts.Port == 465
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls
            : SecureSocketOptions.None;

        using var client = new SmtpClient();

        try
        {
            logger.LogDebug("SMTP connecting to {Host}:{Port} Secure={Secure}.", host, opts.Port, secure);
            await client.ConnectAsync(host, opts.Port, secure, cancellationToken);

            var user = opts.Username?.Trim();
            if (!string.IsNullOrEmpty(user))
            {
                logger.LogDebug("SMTP authenticating as {User}.", user);
                await client.AuthenticateAsync(user, opts.Password ?? string.Empty, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (SmtpCommandException ex)
        {
            logger.LogError(
                ex,
                "SMTP command error after connect. StatusCode={StatusCode} ErrorCode={ErrorCode} Mailbox={Mailbox}",
                ex.StatusCode,
                ex.ErrorCode,
                ex.Mailbox);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SMTP failure (connect, auth, or send) for host {Host}, port {Port}.", host, opts.Port);
            throw;
        }
    }
}
