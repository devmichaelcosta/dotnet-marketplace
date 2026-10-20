namespace Marketplace.Api.Modules.Auth.Website.Shared;

public sealed class SmtpEmailOptions
{
    public const string SectionName = "Email:Smtp";

    public string? Host { get; init; }
    public int Port { get; init; } = 587;
    public bool EnableSsl { get; init; } = true;
    public string? FromAddress { get; init; }
    public string? FromDisplayName { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
}
