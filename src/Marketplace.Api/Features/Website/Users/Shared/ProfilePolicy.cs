namespace Marketplace.Api.Features.Website.Users.Shared;

public static class ProfilePolicy
{
    public static Dictionary<string, string[]>? Validate(ProfileRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors["name"] = ["Nome obrigatorio."];
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            errors["lastName"] = ["Sobrenome obrigatorio."];
        }

        var cpf = NormalizeDocument(request.Cpf);
        if (!string.IsNullOrWhiteSpace(cpf) && cpf.Length != 11)
        {
            errors["cpf"] = ["CPF deve conter 11 digitos."];
        }

        return errors.Count == 0 ? null : errors;
    }

    public static string? NormalizeDocument(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var digits = new string(value.Where(char.IsDigit).ToArray());
        return string.IsNullOrWhiteSpace(digits) ? null : digits;
    }
}
