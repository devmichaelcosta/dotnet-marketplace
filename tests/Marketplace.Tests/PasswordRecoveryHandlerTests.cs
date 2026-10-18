using System.Text;
using Marketplace.Api.Modules.Auth.Website.PasswordRecovery.Confirm;
using Marketplace.Api.Modules.Auth.Website.PasswordRecovery.Request;
using Marketplace.Api.Modules.Auth.Website.Shared;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Marketplace.Tests;

public sealed class PasswordRecoveryHandlerTests
{
    [Fact]
    public async Task Request_for_unknown_login_returns_generic_response_without_sending_email()
    {
        var identity = new FakePasswordResetIdentity();
        var emailSender = new FakeEmailSender();
        var handler = CreateRequestHandler(identity, emailSender);

        var result = await handler.HandleAsync("unknown-account", CancellationToken.None);

        Assert.Equal(RequestPasswordResetHandler.GenericResponse, result.Message);
        Assert.Empty(emailSender.Messages);
    }

    [Fact]
    public async Task Request_for_existing_account_sends_url_encoded_token_and_generic_response()
    {
        var identity = CreateIdentity();
        var emailSender = new FakeEmailSender();
        var handler = CreateRequestHandler(identity, emailSender);

        var result = await handler.HandleAsync(identity.Account!.Login, CancellationToken.None);

        Assert.Equal(RequestPasswordResetHandler.GenericResponse, result.Message);
        var email = Assert.Single(emailSender.Messages);
        Assert.Equal(identity.Account.Email, email.Recipient);
        Assert.Contains("reset-password?login=michael&token=", email.Body);
        Assert.DoesNotContain(identity.GeneratedToken, email.Body);
    }

    [Fact]
    public async Task Invalid_or_expired_token_is_rejected_without_exposing_account_state()
    {
        var identity = CreateIdentity();
        identity.AcceptResetToken = false;
        var handler = new ResetPasswordHandler(identity);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(identity.GeneratedToken));

        var succeeded = await handler.HandleAsync(
            new ResetPasswordRequest(identity.Account!.Login, encodedToken, "NewPassword123"),
            CancellationToken.None);

        Assert.False(succeeded);
    }

    [Fact]
    public async Task Successful_reset_invalidates_the_token_for_reuse()
    {
        var identity = CreateIdentity();
        var handler = new ResetPasswordHandler(identity);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(identity.GeneratedToken));
        var request = new ResetPasswordRequest(identity.Account!.Login, encodedToken, "NewPassword123");

        var firstAttempt = await handler.HandleAsync(request, CancellationToken.None);
        var replayAttempt = await handler.HandleAsync(request, CancellationToken.None);

        Assert.True(firstAttempt);
        Assert.False(replayAttempt);
    }

    [Fact]
    public async Task Reset_with_unknown_login_is_rejected()
    {
        var handler = new ResetPasswordHandler(new FakePasswordResetIdentity());
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("reset-token"));

        var succeeded = await handler.HandleAsync(
            new ResetPasswordRequest("unknown-account", encodedToken, "NewPassword123"),
            CancellationToken.None);

        Assert.False(succeeded);
    }

    private static FakePasswordResetIdentity CreateIdentity() =>
        new()
        {
            Account = new PasswordResetAccount(Guid.NewGuid(), "michael", "michael@example.test")
        };

    private static RequestPasswordResetHandler CreateRequestHandler(
        FakePasswordResetIdentity identity,
        FakeEmailSender emailSender) =>
        new(
            identity,
            emailSender,
            Options.Create(new PasswordResetOptions { PublicWebBaseUrl = "https://shop.example.test/" }),
            NullLogger<RequestPasswordResetHandler>.Instance);
}