using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace Marketplace.Web.Services;

public sealed class MarketplaceApiClient(HttpClient http, ClientState state)
{
    public string AssetUrl(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        if (Uri.TryCreate(path, UriKind.Absolute, out var absolute))
        {
            return absolute.ToString();
        }

        return new Uri(http.BaseAddress!, path.TrimStart('/')).ToString();
    }

    public async Task<HomeResponse?> GetHomeAsync(CancellationToken cancellationToken = default) =>
        await http.GetFromJsonAsync<HomeResponse>("api/catalog/home", cancellationToken);

    public async Task<StateOption[]> GetStatesAsync(CancellationToken cancellationToken = default) =>
        await http.GetFromJsonAsync<StateOption[]>("api/catalog/states", cancellationToken) ?? [];

    public async Task<ProductSearchResponse?> SearchProductsAsync(string? search, int? categoryId = null, int page = 1, CancellationToken cancellationToken = default)
    {
        var url = $"api/catalog/products?page={page}";
        if (!string.IsNullOrWhiteSpace(search))
        {
            url += $"&search={Uri.EscapeDataString(search)}";
        }

        if (categoryId is not null)
        {
            url += $"&categoryId={categoryId.Value}";
        }

        return await http.GetFromJsonAsync<ProductSearchResponse>(url, cancellationToken);
    }

    public async Task<ProductDetailsResponse?> GetProductDetailsAsync(int id, CancellationToken cancellationToken = default) =>
        await http.GetFromJsonAsync<ProductDetailsResponse>($"api/catalog/products/{id}", cancellationToken);

    public async Task LikeProductAsync(int id, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Post, $"api/products/{id}/like");
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task UnlikeProductAsync(int id, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Delete, $"api/products/{id}/like");
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<ProductSummary[]> GetLikedProductsAsync(CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, "api/products/liked");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        return await response.Content.ReadFromJsonAsync<ProductSummary[]>(cancellationToken) ?? [];
    }

    public async Task SubmitRatingAsync(int productId, RatingRequest rating, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Post, $"api/products/{productId}/ratings");
        request.Content = JsonContent.Create(rating);
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<CartResponse?> GetCartAsync(CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, "api/cart");
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await ReadCartAsync(response, cancellationToken);
    }

    public async Task<CartResponse?> AddToCartAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await AddToCartAsync(productId, 1, cancellationToken);
    }

    public async Task<CartResponse?> AddToCartAsync(int productId, int quantity, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Post, "api/cart/items");
        request.Content = JsonContent.Create(new { ProductId = productId, Quantity = quantity });
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await ReadCartAsync(response, cancellationToken);
    }

    public async Task UpdateCartItemAsync(int productId, int quantity, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Put, $"api/cart/items/{productId}");
        request.Content = JsonContent.Create(new { Quantity = quantity });
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveCartItemAsync(int productId, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Delete, $"api/cart/items/{productId}");
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<int?> CheckoutAsync(CheckoutRequest checkout, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Post, "api/cart/checkout");
        request.Content = JsonContent.Create(checkout);
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<CheckoutResponse>(cancellationToken);
        return result?.Id;
    }

    public async Task<LoginResponse?> LoginAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        using var response = await http.PostAsJsonAsync("api/auth/login", new { Login = login, Password = password }, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken);
        if (result is not null)
        {
            state.SignIn(result.Token, result.User.UserName, result.User.Roles);
        }

        return result;
    }

    public async Task<RegisterResult> RegisterAsync(RegisterRequest register, bool seller, CancellationToken cancellationToken = default)
    {
        var url = seller ? "api/auth/register-seller" : "api/auth/register";
        using var response = await http.PostAsJsonAsync(url, register, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return new RegisterResult(true, "Conta criada. Agora faca login.");
        }

        return new RegisterResult(false, await ReadProblemMessageAsync(response, cancellationToken));
    }

    private static async Task<string> ReadProblemMessageAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var fallback = "Nao foi possivel criar a conta.";
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return fallback;
        }

        try
        {
            using var document = JsonDocument.Parse(content);
            if (document.RootElement.TryGetProperty("errors", out var errors))
            {
                foreach (var property in errors.EnumerateObject())
                {
                    if (property.Value.ValueKind == JsonValueKind.Array)
                    {
                        var message = property.Value.EnumerateArray()
                            .Select(item => item.GetString())
                            .FirstOrDefault(item => !string.IsNullOrWhiteSpace(item));

                        if (!string.IsNullOrWhiteSpace(message))
                        {
                            return message!;
                        }
                    }
                }
            }

            if (document.RootElement.TryGetProperty("title", out var title))
            {
                var message = title.GetString();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    return message;
                }
            }
        }
        catch (JsonException)
        {
            return fallback;
        }

        return fallback;
    }

    public async Task<UserProfile?> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, "api/users/me");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<UserProfile>(cancellationToken);
    }

    public async Task SaveProfileAsync(ProfileRequest profile, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Put, "api/users/me");
        request.Content = JsonContent.Create(profile);
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAddressAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Delete, $"api/users/me/addresses/{id}");
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<AdminProductSearchResponse?> GetAdminProductsAsync(CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, "api/admin/products?page=1");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<AdminProductSearchResponse>(cancellationToken);
    }

    public async Task<AdminProductDetails?> GetAdminProductAsync(int id, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, $"api/admin/products/{id}");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<AdminProductDetails>(cancellationToken);
    }

    public async Task<int?> SaveProductAsync(AdminProductRequest product, CancellationToken cancellationToken = default)
    {
        var method = product.Id is null ? HttpMethod.Post : HttpMethod.Put;
        var url = product.Id is null ? "api/admin/products" : $"api/admin/products/{product.Id}";
        using var request = NewRequest(method, url);
        request.Content = JsonContent.Create(new
        {
            product.UserId,
            product.SubCategoryId,
            product.Title,
            product.Description,
            product.Price,
            product.Stock,
            product.Offer,
            product.Sku,
            product.Images,
            product.Attributes
        });
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<SaveProductResponse>(cancellationToken);
        return result?.Id;
    }

    public async Task DeleteProductAsync(int id, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Delete, $"api/admin/products/{id}");
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<ProductImportCreated?> UploadProductImportAsync(IBrowserFile file, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(file.OpenReadStream(5 * 1024 * 1024, cancellationToken));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);

        using var request = NewRequest(HttpMethod.Post, "api/admin/product-imports");
        request.Content = content;
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ProductImportCreated>(cancellationToken);
    }

    public async Task<PagedResult<ProductImportJob>?> GetProductImportsAsync(
        string? search = null,
        string? status = null,
        string? sort = null,
        string? direction = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var url = $"api/admin/product-imports?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search))
        {
            url += $"&search={Uri.EscapeDataString(search)}";
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            url += $"&status={Uri.EscapeDataString(status)}";
        }

        if (!string.IsNullOrWhiteSpace(sort))
        {
            url += $"&sort={Uri.EscapeDataString(sort)}";
        }

        if (!string.IsNullOrWhiteSpace(direction))
        {
            url += $"&direction={Uri.EscapeDataString(direction)}";
        }

        using var request = NewRequest(HttpMethod.Get, url);
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<PagedResult<ProductImportJob>>(cancellationToken);
    }

    public async Task<ProductImportDetails?> GetProductImportAsync(int id, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, $"api/admin/product-imports/{id}");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ProductImportDetails>(cancellationToken);
    }

    public async Task<PagedResult<ProductImportItem>?> GetProductImportItemsAsync(
        int id,
        string? search = null,
        string? status = null,
        string? sort = null,
        string? direction = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var url = $"api/admin/product-imports/{id}/items?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search))
        {
            url += $"&search={Uri.EscapeDataString(search)}";
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            url += $"&status={Uri.EscapeDataString(status)}";
        }

        if (!string.IsNullOrWhiteSpace(sort))
        {
            url += $"&sort={Uri.EscapeDataString(sort)}";
        }

        if (!string.IsNullOrWhiteSpace(direction))
        {
            url += $"&direction={Uri.EscapeDataString(direction)}";
        }

        using var request = NewRequest(HttpMethod.Get, url);
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<PagedResult<ProductImportItem>>(cancellationToken);
    }

    public async Task<FileDownload?> DownloadProductImportTemplateAsync(CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, "api/admin/product-imports/template");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return new FileDownload(
            await response.Content.ReadAsByteArrayAsync(cancellationToken),
            response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream",
            "product-import-template.xlsx");
    }

    public async Task<FileDownload?> DownloadProductImportFileAsync(int id, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, $"api/admin/product-imports/{id}/file");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
            ?? response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
            ?? "product-import.xlsx";
        return new FileDownload(
            await response.Content.ReadAsByteArrayAsync(cancellationToken),
            response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream",
            fileName);
    }

    public async Task SaveSimilarProductsAsync(int id, int[] productIds, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Post, $"api/admin/products/{id}/similar-products");
        request.Content = JsonContent.Create(new { ProductIds = productIds });
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<AdminCategory[]> GetAdminCategoriesAsync(CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, "api/admin/categories");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        return await response.Content.ReadFromJsonAsync<AdminCategory[]>(cancellationToken) ?? [];
    }

    public async Task SaveCategoryAsync(AdminCategoryRequest category, CancellationToken cancellationToken = default)
    {
        var method = category.Id is null ? HttpMethod.Post : HttpMethod.Put;
        var url = category.Id is null ? "api/admin/categories" : $"api/admin/categories/{category.Id}";
        using var request = NewRequest(method, url);
        request.Content = JsonContent.Create(new { category.Title, category.Image });
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Delete, $"api/admin/categories/{id}");
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<AdminSubCategory[]> GetAdminSubCategoriesAsync(CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, "api/admin/subcategories");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        return await response.Content.ReadFromJsonAsync<AdminSubCategory[]>(cancellationToken) ?? [];
    }

    public async Task SaveSubCategoryAsync(AdminSubCategoryRequest subCategory, CancellationToken cancellationToken = default)
    {
        var method = subCategory.Id is null ? HttpMethod.Post : HttpMethod.Put;
        var url = subCategory.Id is null ? "api/admin/subcategories" : $"api/admin/subcategories/{subCategory.Id}";
        using var request = NewRequest(method, url);
        request.Content = JsonContent.Create(new { subCategory.CategoryId, subCategory.Title });
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteSubCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Delete, $"api/admin/subcategories/{id}");
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<AdminAttribute[]> GetAdminAttributesAsync(CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, "api/admin/attributes");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        return await response.Content.ReadFromJsonAsync<AdminAttribute[]>(cancellationToken) ?? [];
    }

    public async Task SaveAttributeAsync(AdminAttributeRequest attribute, CancellationToken cancellationToken = default)
    {
        var method = attribute.Id is null ? HttpMethod.Post : HttpMethod.Put;
        var url = attribute.Id is null ? "api/admin/attributes" : $"api/admin/attributes/{attribute.Id}";
        using var request = NewRequest(method, url);
        request.Content = JsonContent.Create(new { attribute.Name });
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAttributeAsync(int id, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Delete, $"api/admin/attributes/{id}");
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<AdminUser[]> GetAdminUsersAsync(CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, "api/admin/users");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        return await response.Content.ReadFromJsonAsync<AdminUser[]>(cancellationToken) ?? [];
    }

    public async Task SaveUserAsync(AdminUserRequest user, CancellationToken cancellationToken = default)
    {
        var method = user.Id is null ? HttpMethod.Post : HttpMethod.Put;
        var url = user.Id is null ? "api/admin/users" : $"api/admin/users/{user.Id}";
        using var request = NewRequest(method, url);
        request.Content = JsonContent.Create(new { user.Name, user.LastName, user.Login, user.Password, user.Cpf, user.Role });
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Delete, $"api/admin/users/{id}");
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<AdminSeller[]> GetAdminSellersAsync(CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, "api/admin/sellers");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        return await response.Content.ReadFromJsonAsync<AdminSeller[]>(cancellationToken) ?? [];
    }

    public async Task<AdminSeller?> GetAdminSellerAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, $"api/admin/sellers/{id}");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<AdminSeller>(cancellationToken);
    }

    public async Task SaveSellerAsync(AdminSellerRequest seller, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Put, $"api/admin/sellers/{seller.Id}");
        request.Content = JsonContent.Create(seller);
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task CreateSellerAsync(AdminSellerCreateRequest seller, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Post, "api/admin/sellers");
        request.Content = JsonContent.Create(seller);
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteSellerAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Delete, $"api/admin/sellers/{id}");
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<AdminCarouselImage[]> GetCarouselAsync(CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, "api/admin/carousel");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        return await response.Content.ReadFromJsonAsync<AdminCarouselImage[]>(cancellationToken) ?? [];
    }

    public async Task SaveCarouselAsync(AdminCarouselRequest carousel, CancellationToken cancellationToken = default)
    {
        var method = carousel.Id is null ? HttpMethod.Post : HttpMethod.Put;
        var url = carousel.Id is null ? "api/admin/carousel" : $"api/admin/carousel/{carousel.Id}";
        using var request = NewRequest(method, url);
        request.Content = JsonContent.Create(new { carousel.FileName, carousel.SortOrder });
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCarouselAsync(int id, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Delete, $"api/admin/carousel/{id}");
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<UploadResult?> UploadImageAsync(string scope, IBrowserFile file, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(file.OpenReadStream(5 * 1024 * 1024, cancellationToken));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);

        using var request = NewRequest(HttpMethod.Post, $"api/admin/uploads/{scope}");
        request.Content = content;
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<UploadResult>(cancellationToken);
    }

    public async Task<PendingRating[]> GetPendingRatingsAsync(CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, "api/admin/ratings/pending");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        return await response.Content.ReadFromJsonAsync<PendingRating[]>(cancellationToken) ?? [];
    }

    public async Task ApproveRatingAsync(int id, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Post, $"api/admin/ratings/{id}/approve");
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<OrderSummary[]> GetOrdersAsync(CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, "api/orders");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        return await response.Content.ReadFromJsonAsync<OrderSummary[]>(cancellationToken) ?? [];
    }

    public async Task<OrderDetails?> GetOrderDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, $"api/orders/{id}");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<OrderDetails>(cancellationToken);
    }

    public async Task<AdminOrderSummary[]> GetAdminOrdersAsync(string? search = null, CancellationToken cancellationToken = default)
    {
        var url = "api/admin/orders";
        if (!string.IsNullOrWhiteSpace(search))
        {
            url += $"?search={Uri.EscapeDataString(search)}";
        }

        using var request = NewRequest(HttpMethod.Get, url);
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        return await response.Content.ReadFromJsonAsync<AdminOrderSummary[]>(cancellationToken) ?? [];
    }

    public async Task<AdminOrderDetails?> GetAdminOrderDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        using var request = NewRequest(HttpMethod.Get, $"api/admin/orders/{id}");
        using var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<AdminOrderDetails>(cancellationToken);
    }

    private async Task<CartResponse?> ReadCartAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var cart = await response.Content.ReadFromJsonAsync<CartResponse>(cancellationToken);
        if (cart is not null)
        {
            state.UpdateCartId(cart.CartId);
        }

        return cart;
    }

    private HttpRequestMessage NewRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Add("X-Cart-Id", state.CartId);
        if (!string.IsNullOrWhiteSpace(state.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", state.Token);
        }

        return request;
    }
}

public sealed record HomeResponse(CarouselImage[] Carousel, Category[] Categories, ProductSummary[] Offers);
public sealed record CarouselImage(int Id, string FileName, int SortOrder);
public sealed record StateOption(int Id, string Name, string Abbreviation);
public sealed record Category(int Id, string Title, string? Image, SubCategory[]? SubCategories);
public sealed record SubCategory(int Id, int CategoryId, string Title);
public sealed record AdminCategory(int Id, string Title, string? Image, SubCategoryOption[] SubCategories);
public sealed record SubCategoryOption(int Id, string Title);
public sealed record AdminSubCategory(int Id, int CategoryId, string Title, string Category);
public sealed record AdminAttribute(int Id, string Name);
public sealed record AdminCategoryRequest(int? Id, string Title, string? Image);
public sealed record AdminSubCategoryRequest(int? Id, int CategoryId, string Title);
public sealed record AdminAttributeRequest(int? Id, string Name);
public sealed record AdminProductSearchResponse(AdminProductSummary[] Items, int Total, int Page, int PageSize);
public sealed record AdminProductSummary(int Id, string Title, decimal Price, int Stock, bool Offer, string Sku, string Seller);
public sealed record AdminProductDetails(
    int Id,
    Guid UserId,
    int? SubCategoryId,
    string Title,
    string Description,
    decimal Price,
    int Stock,
    bool Offer,
    string Sku,
    string[] Images,
    ProductAttributeValueRequest[] Attributes,
    int[] SimilarProductIds);
public sealed record AdminProductRequest(
    int? Id,
    Guid? UserId,
    int? SubCategoryId,
    string Title,
    string Description,
    decimal Price,
    int Stock,
    bool Offer,
    string Sku,
    string[] Images,
    ProductAttributeValueRequest[] Attributes);
public sealed record SaveProductResponse(int Id);
public sealed record ProductImportCreated(int JobId);
public sealed record PagedResult<T>(T[] Items, int Total, int Page, int PageSize);
public sealed record ProductImportJob(
    int Id,
    string OriginalFileName,
    string ImportedByName,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? FinishedAt,
    long? DurationMs,
    int TotalRows,
    int SkuCount,
    int CreatedCount,
    int UpdatedCount,
    int ErrorCount,
    string SummaryMessage);
public sealed record ProductImportDetails(
    int Id,
    string OriginalFileName,
    string ImportedByName,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? FinishedAt,
    long? DurationMs,
    int TotalRows,
    int SkuCount,
    int CreatedCount,
    int UpdatedCount,
    int ErrorCount,
    string SummaryMessage,
    long FileSizeBytes);
public sealed record ProductImportItem(
    int Id,
    int RowNumber,
    string Sku,
    string Title,
    string Status,
    string ErrorMessage,
    int? ProductId,
    string DownloadedImages,
    string ImportedAttributes);
public sealed record FileDownload(byte[] Bytes, string ContentType, string FileName);
public sealed record ProductAttributeValueRequest(int AttributeId, string Value);
public sealed record AdminUser(Guid Id, string Login, string Name, string LastName, string? Cpf, string Role);
public sealed record AdminUserRequest(Guid? Id, string Name, string LastName, string Login, string? Password, string? Cpf, string Role);
public sealed record AdminSeller(
    Guid Id,
    Guid UserId,
    string Name,
    string LastName,
    string? Cpf,
    int? Age,
    string? Email,
    DateOnly? DateOfBirth,
    string? Website,
    string? Company,
    string? Cnpj,
    string? BranchOfActivity,
    string? FantasyName);
public sealed record AdminSellerRequest(
    Guid Id,
    string Name,
    string LastName,
    string? Cpf,
    int? Age,
    string? Email,
    DateOnly? DateOfBirth,
    string? Website,
    string? Company,
    string? Cnpj,
    string? BranchOfActivity,
    string? FantasyName);
public sealed record AdminSellerCreateRequest(
    string Name,
    string LastName,
    string Login,
    string Password,
    string? Cpf,
    int? Age,
    string? Email,
    DateOnly? DateOfBirth,
    string? Website,
    string? Company,
    string? Cnpj,
    string? BranchOfActivity,
    string? FantasyName);
public sealed record AdminCarouselImage(int Id, string FileName, int SortOrder);
public sealed record AdminCarouselRequest(int? Id, string FileName, int SortOrder);
public sealed record UploadResult(string FileName, string Url);
public sealed record ProductSearchResponse(ProductSummary[] Items, int Total, int Page, int PageSize);
public sealed record ProductSummary(int Id, string Title, decimal Price, int Stock, bool Offer, string? Image, string? Seller);
public sealed record ProductDetailsResponse(ProductDetails Product, ProductSummary[] SimilarProducts);
public sealed record ProductDetails(
    int Id,
    string Title,
    string Description,
    decimal Price,
    int Stock,
    bool Offer,
    string Sku,
    string Seller,
    string? Category,
    string? SubCategory,
    string[] Images,
    ProductAttributeValue[] Attributes,
    ProductRating[] Ratings);
public sealed record ProductAttributeValue(string Attribute, string Value);
public sealed record ProductRating(string Title, string Description, string Rating, bool Recommended);
public sealed record RatingRequest(string Title, string Description, bool Recommended, string Rating);
public sealed record PendingRating(
    int Id,
    int ProductId,
    string ProductTitle,
    string UserName,
    string Title,
    string Description,
    string Rating,
    bool Recommended,
    DateTimeOffset CreatedAt);
public sealed record CartResponse(string CartId, CartItem[] Items, decimal SubTotal, decimal Shipping, decimal Total);
public sealed record CartItem(int ProductId, string Title, int Quantity, decimal UnitPrice, decimal SubTotal, string? Image);
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
public sealed record CheckoutResponse(int Id);
public sealed record LoginResponse(string Token, LoginUser User);
public sealed record LoginUser(Guid Id, string UserName, string Name, string[] Roles);
public sealed record RegisterRequest(string Name, string LastName, string Login, string Password, string? Cpf);
public sealed record RegisterResult(bool Succeeded, string Message);
public sealed record UserProfile(Guid Id, string Login, string Name, string LastName, string? Cpf, string[] Roles, UserAddress[] Addresses);
public sealed record UserAddress(Guid Id, int StateId, string State, string Street, string Cep, string Neighborhood, string City, string? Complement);
public sealed record ProfileRequest(string Name, string LastName, string? Cpf, AddressRequest[] Addresses);
public sealed record AddressRequest(int StateId, string Street, string Cep, string Neighborhood, string City, string? Complement);
public sealed record OrderSummary(int Id, decimal Total, DateTimeOffset CreatedAt, string Name, string City);
public sealed record AdminOrderSummary(
    int Id,
    decimal Total,
    DateTimeOffset CreatedAt,
    string Name,
    string City,
    string UserName,
    string Login);
public sealed record OrderDetails(
    int Id,
    decimal Total,
    DateTimeOffset CreatedAt,
    string Name,
    string Address,
    string Neighborhood,
    string Cep,
    string City,
    string? Complement,
    string State,
    OrderItem[] Items);
public sealed record AdminOrderDetails(
    int Id,
    decimal Total,
    DateTimeOffset CreatedAt,
    string Name,
    string Login,
    string Address,
    string Neighborhood,
    string Cep,
    string City,
    string? Complement,
    string State,
    OrderItem[] Items);
public sealed record OrderItem(int ProductId, string Title, int Quantity, decimal UnitPrice, decimal SubTotal);
