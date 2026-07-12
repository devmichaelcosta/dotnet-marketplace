namespace Marketplace.Api.Features.Products.Admin.Create;

public static class CreateProductEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateProductRequest request,
            HttpContext http,
            CreateProductHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(request, http, cancellationToken));
    }
}

public sealed class CreateProductRequestValidator
{
    public Dictionary<string, string[]> Validate(CreateProductRequest request)
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