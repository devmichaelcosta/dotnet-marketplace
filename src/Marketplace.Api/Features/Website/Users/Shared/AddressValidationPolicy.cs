namespace Marketplace.Api.Features.Website.Users.Shared;

internal static class AddressValidationPolicy
{
    public static Dictionary<string, string[]>? Validate(AddressRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.StateId <= 0)
        {
            errors["stateId"] = ["Estado obrigatorio."];
        }

        if (string.IsNullOrWhiteSpace(request.Street))
        {
            errors["street"] = ["Rua obrigatoria."];
        }

        if (string.IsNullOrWhiteSpace(request.Cep))
        {
            errors["cep"] = ["CEP obrigatorio."];
        }

        if (string.IsNullOrWhiteSpace(request.Neighborhood))
        {
            errors["neighborhood"] = ["Bairro obrigatorio."];
        }

        if (string.IsNullOrWhiteSpace(request.City))
        {
            errors["city"] = ["Cidade obrigatoria."];
        }

        return errors.Count == 0 ? null : errors;
    }
}
