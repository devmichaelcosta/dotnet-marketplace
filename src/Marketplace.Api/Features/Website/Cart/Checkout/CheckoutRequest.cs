namespace Marketplace.Api.Features.Website.Cart.Checkout;

public sealed record CheckoutRequest(
    string Name,
    string CardOwnerName,
    string ExpirationDate,
    string Address,
    string Neighborhood,
    string Cep,
    string City,
    string Cpf,
    int StateId,
    string? Complement);
