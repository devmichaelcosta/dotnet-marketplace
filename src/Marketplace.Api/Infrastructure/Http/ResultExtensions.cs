using Microsoft.AspNetCore.Http.HttpResults;

namespace Marketplace.Api.Infrastructure.Http;

public static class ResultExtensions
{
    public static Results<Ok<T>, NotFound> ToOkOrNotFound<T>(this T? result)
    {
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }
}
