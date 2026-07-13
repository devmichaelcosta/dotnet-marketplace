namespace Marketplace.Api.Features.Website.Produto.CreateRating;

public sealed record CreateProductRatingRequest(string Title, string Description, bool Recommended, string Rating);

