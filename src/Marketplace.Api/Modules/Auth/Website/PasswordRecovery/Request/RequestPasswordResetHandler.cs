using Marketplace.Api.Modules.Auth.Website.Shared;
using Microsoft.Extensions.Options;

namespace Marketplace.Api.Modules.Auth.Website.PasswordRecovery.Request;

public sealed class RequestPasswordResetHandler(
    IPasswordResetIdentity identity,
    IEmailSender emailSender,
    IOptions<PasswordResetOptions> options,
    ILogger<RequestPasswordResetHandler> logger)
{
    public const string GenericResponse =
        "Se a conta estiver cadastrada e puder receber mensagens, enviaremos as instrucoes para o e-mail associado.";

    public async Task<RequestPasswordResetResponse> HandleAsync(string? login, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            return new RequestPasswordResetResponse(GenericResponse);
        }

        var account = await identity.FindByLoginAsync(login.Trim(), cancellationToken);
        if (account is null || string.IsNullOrWhiteSpace(account.Email))
        {
            return new RequestPasswordResetResponse(GenericResponse);
        }

        try
        {
            var token = await identity.GenerateTokenAsync(account.Id, cancellationToken);
            var link = PasswordResetLinkBuilder.Build(options.Value.PublicWebBaseUrl, account.Login, token);
            var body = $"Use este link para redefinir sua senha. O link expira em uma hora:\n\n{link}\n\nSe voce nao solicitou a redefinicao, ignore esta mensagem.";
            await emailSender.SendAsync(account.Email, "Redefinicao de senha", body, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError("Password reset email delivery failed ({ExceptionType}).", exception.GetType().Name);
        }

        return new RequestPasswordResetResponse(GenericResponse);
    }
}