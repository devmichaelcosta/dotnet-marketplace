using Marketplace.Api.Features.Website.Cart.Checkout;

namespace Marketplace.Tests;

public sealed class CheckoutValidationPolicyTests
{
    [Fact]
    public void Valid_checkout_request_has_no_validation_errors()
    {
        var errors = CheckoutValidationPolicy.Validate(CreateRequest());

        Assert.Null(errors);
    }

    [Fact]
    public void Checkout_requires_shipping_identity_and_valid_documents()
    {
        var errors = CheckoutValidationPolicy.Validate(new CheckoutRequest(
            Name: "",
            Address: "",
            Neighborhood: "",
            Cep: "123",
            City: "",
            Cpf: "123",
            StateId: 0,
            Complement: new string('x', 251)));

        Assert.NotNull(errors);
        Assert.Contains("name", errors.Keys);
        Assert.Contains("address", errors.Keys);
        Assert.Contains("neighborhood", errors.Keys);
        Assert.Contains("city", errors.Keys);
        Assert.Contains("cep", errors.Keys);
        Assert.Contains("cpf", errors.Keys);
        Assert.Contains("stateId", errors.Keys);
        Assert.Contains("complement", errors.Keys);
    }

    [Fact]
    public void Checkout_accepts_formatted_cpf_and_cep()
    {
        var request = CreateRequest() with
        {
            Cpf = "123.456.789-01",
            Cep = "12345-678"
        };

        Assert.Null(CheckoutValidationPolicy.Validate(request));
    }

    private static CheckoutRequest CreateRequest() => new(
        Name: "Cliente",
        Address: "Rua A, 10",
        Neighborhood: "Centro",
        Cep: "12345678",
        City: "Sao Paulo",
        Cpf: "12345678901",
        StateId: 1,
        Complement: null);
}
