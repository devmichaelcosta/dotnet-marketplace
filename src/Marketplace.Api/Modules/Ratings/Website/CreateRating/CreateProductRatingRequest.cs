namespace Marketplace.Api.Modules.Ratings.Website.CreateRating;

public sealed record CreateProductRatingRequest(string Title, string Description, bool Recommended, string Rating);

