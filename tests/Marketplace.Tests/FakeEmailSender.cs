using Marketplace.Api.Modules.Auth.Website.Shared;

namespace Marketplace.Tests;

public sealed class FakeEmailSender : IEmailSender
{
    public List<(string Recipient, string Subject, string Body)> Messages { get; } = [];

    public Task SendAsync(string recipient, string subject, string body, CancellationToken cancellationToken)
    {
        Messages.Add((recipient, subject, body));
        return Task.CompletedTask;
    }
}