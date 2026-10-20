namespace Marketplace.Api.Modules.Auth.Website.Shared;

public interface IEmailSender
{
    Task SendAsync(string recipient, string subject, string body, CancellationToken cancellationToken);
}
