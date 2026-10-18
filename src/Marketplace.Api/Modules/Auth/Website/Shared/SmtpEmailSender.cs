using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;

namespace Marketplace.Api.Modules.Auth.Website.Shared;

public sealed class SmtpEmailSender(IOptions<SmtpEmailOptions> options) : IEmailSender
{
    public async Task SendAsync(string recipient, string subject, string body, CancellationToken cancellationToken)
    {
        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.Host)
            || string.IsNullOrWhiteSpace(settings.FromAddress)
            || settings.Port is < 1 or > 65535)
        {
            throw new InvalidOperationException("SMTP email delivery is not configured.");
        }

        using var client = new SmtpClient(settings.Host, settings.Port)
        {
            EnableSsl = settings.EnableSsl,
            UseDefaultCredentials = false
        };

        if (!string.IsNullOrWhiteSpace(settings.Username))
        {
            client.Credentials = new NetworkCredential(settings.Username, settings.Password);
        }

        using var message = new MailMessage
        {
            From = new MailAddress(settings.FromAddress, settings.FromDisplayName),
            Subject = subject,
            SubjectEncoding = Encoding.UTF8,
            Body = body,
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = false
        };
        message.To.Add(new MailAddress(recipient));

        await client.SendMailAsync(message, cancellationToken);
    }
}
