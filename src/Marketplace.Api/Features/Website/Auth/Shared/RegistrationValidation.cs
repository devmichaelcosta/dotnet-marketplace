namespace Marketplace.Api.Features.Website.Auth.Shared;

internal static class RegistrationValidation
{
    public static Dictionary<string, string[]>? Validate(RegisterRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors["name"] = ["Nome obrigatorio."];
        }

        if (string.IsNullOrWhiteSpace(request.Login) || request.Login.Trim().Length < 3)
        {
            errors["login"] = ["Login deve ter pelo menos 3 caracteres."];
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
        {
            errors["password"] = ["Senha deve ter pelo menos 6 caracteres."];
        }

        var cpf = DocumentNormalizer.Normalize(request.Cpf);
        if (!string.IsNullOrWhiteSpace(cpf) && cpf.Length != 11)
        {
            errors["cpf"] = ["CPF deve conter 11 digitos."];
        }

        return errors.Count == 0 ? null : errors;
    }
}
