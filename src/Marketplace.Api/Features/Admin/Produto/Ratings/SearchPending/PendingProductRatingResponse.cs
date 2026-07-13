namespace Marketplace.Api.Features.Admin.Produto.Ratings.SearchPending;

public sealed record PendingProductRatingResponse(
    int Id,
    int ProductId,
    string ProductTitle,
    string UserName,
    string Title,
    string Description,
    string Rating,
    bool Recommended,
    DateTimeOffset CreatedAt);

