namespace Marketplace.Api.Features.Website.Cart.Checkout;

public static class CheckoutValidationPolicy
{
    public static Dictionary<string, string[]>? Validate(CheckoutRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        AddRequired(errors, "name", request.Name, "Nome obrigatorio.", 160);
        AddRequired(errors, "address", request.Address, "Endereco obrigatorio.", 250);
        AddRequired(errors, "neighborhood", request.Neighborhood, "Bairro obrigatorio.", 120);
        AddRequired(errors, "city", request.City, "Cidade obrigatoria.", 120);

        if (string.IsNullOrWhiteSpace(request.Cpf))
        {
            errors["cpf"] = ["CPF obrigatorio."];
        }
        else if (NormalizeDigits(request.Cpf).Length != 11)
        {
            errors["cpf"] = ["CPF deve conter 11 digitos."];
        }

        if (string.IsNullOrWhiteSpace(request.Cep))
        {
            errors["cep"] = ["CEP obrigatorio."];
        }
        else if (NormalizeDigits(request.Cep).Length != 8)
        {
            errors["cep"] = ["CEP deve conter 8 digitos."];
        }

        if (request.StateId <= 0)
        {
            errors["stateId"] = ["Estado obrigatorio."];
        }

        if (!string.IsNullOrWhiteSpace(request.Complement) && request.Complement.Length > 250)
        {
            errors["complement"] = ["Complemento deve ter no maximo 250 caracteres."];
        }

        return errors.Count == 0 ? null : errors;
    }

    private static void AddRequired(
        IDictionary<string, string[]> errors,
        string key,
        string? value,
        string message,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors[key] = [message];
        }
        else if (value.Length > maxLength)
        {
            errors[key] = [$"Campo deve ter no maximo {maxLength} caracteres."];
        }
    }

    private static string NormalizeDigits(string value) =>
        new(value.Where(char.IsDigit).ToArray());
}
