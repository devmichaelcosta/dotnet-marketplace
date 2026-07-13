namespace Marketplace.Api.Features.Admin.Produto.Update;

public sealed class UpdateProductRequestValidator
{
    public Dictionary<string, string[]> Validate(UpdateProductRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            errors["title"] = ["Titulo obrigatorio."];
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            errors["description"] = ["Descricao obrigatoria."];
        }

        if (string.IsNullOrWhiteSpace(request.Sku))
        {
            errors["sku"] = ["SKU obrigatorio."];
        }

        if (request.Price <= 0)
        {
            errors["price"] = ["Preco deve ser maior que zero."];
        }

        if (request.Stock < 0)
        {
            errors["stock"] = ["Estoque nao pode ser negativo."];
        }

        return errors;
    }
}
