namespace Marketplace.Api.Modules.Cart.Website.Checkout;

public sealed record CheckoutRequest(
    string? Name,
    string? Address,
    string? Neighborhood,
    string? Cep,
    string? City,
    string? Cpf,
    int StateId,
    string? Complement);
