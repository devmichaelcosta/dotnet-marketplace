using Marketplace.Api.Domain;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapUserEndpoints();
        app.MapCategoryEndpoints();
        app.MapSubCategoryEndpoints();
        app.MapAttributeEndpoints();
        app.MapSellerEndpoints();
        app.MapCarouselEndpoints();
        app.MapUploadEndpoints();
        return app;
    }

    private static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/users").RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole));

        group.MapGet("/", async (UserManager<ApplicationUser> userManager, CancellationToken cancellationToken) =>
        {
            var users = await userManager.Users.OrderBy(user => user.Name).ToListAsync(cancellationToken);
            var response = new List<UserResponse>();
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                response.Add(UserResponse.From(user, roles.ToArray()));
            }

            return Results.Ok(response);
        });

        group.MapGet("/{id:guid}", async (Guid id, UserManager<ApplicationUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user is null)
            {
                return Results.NotFound();
            }

            var roles = await userManager.GetRolesAsync(user);
            return Results.Ok(UserResponse.From(user, roles.ToArray()));
        });

        group.MapPost("/", async (UserRequest request, UserManager<ApplicationUser> userManager) =>
        {
            var validationErrors = AdminValidationPolicy.ValidateUser(request, passwordRequired: true);
            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var user = new ApplicationUser
            {
                UserName = request.Login.Trim(),
                Email = $"{request.Login.Trim()}@marketplace.local",
                Name = request.Name.Trim(),
                LastName = request.LastName.Trim(),
                Cpf = AdminValidationPolicy.NormalizeDocument(request.Cpf),
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, request.Password!);
            if (!result.Succeeded)
            {
                return Results.ValidationProblem(result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description }));
            }

            await userManager.AddToRoleAsync(user, NormalizeRole(request.Role));
            return Results.Created($"/api/admin/users/{user.Id}", UserResponse.From(user, [NormalizeRole(request.Role)]));
        });

        group.MapPut("/{id:guid}", async (Guid id, UserRequest request, UserManager<ApplicationUser> userManager) =>
        {
            var validationErrors = AdminValidationPolicy.ValidateUser(request, passwordRequired: false);
            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var user = await userManager.FindByIdAsync(id.ToString());
            if (user is null)
            {
                return Results.NotFound();
            }

            user.UserName = request.Login.Trim();
            user.Email = $"{request.Login.Trim()}@marketplace.local";
            user.Name = request.Name.Trim();
            user.LastName = request.LastName.Trim();
            user.Cpf = AdminValidationPolicy.NormalizeDocument(request.Cpf);

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return Results.ValidationProblem(updateResult.Errors.ToDictionary(error => error.Code, error => new[] { error.Description }));
            }

            var currentRoles = await userManager.GetRolesAsync(user);
            if (currentRoles.Count > 0)
            {
                await userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            var role = NormalizeRole(request.Role);
            await userManager.AddToRoleAsync(user, role);
            return Results.Ok(UserResponse.From(user, [role]));
        });

        group.MapPost("/{id:guid}/reset-password", async (Guid id, ResetPasswordRequest request, UserManager<ApplicationUser> userManager) =>
        {
            var validationErrors = AdminPasswordResetPolicy.Validate(request);
            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var user = await userManager.FindByIdAsync(id.ToString());
            if (user is null)
            {
                return Results.NotFound();
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await userManager.ResetPasswordAsync(user, token, request.Password);
            if (!passwordResult.Succeeded)
            {
                return Results.ValidationProblem(passwordResult.Errors.ToDictionary(error => error.Code, error => new[] { error.Description }));
            }

            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (Guid id, UserManager<ApplicationUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user is null)
            {
                return Results.NotFound();
            }

            var result = await userManager.DeleteAsync(user);
            return result.Succeeded
                ? Results.NoContent()
                : Results.ValidationProblem(result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description }));
        });
    }

    private static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/categories").RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole));

        group.MapGet("/", async (MarketplaceDbContext db, CancellationToken cancellationToken) =>
            await db.Categories
                .Include(item => item.SubCategories)
                .OrderBy(item => item.Title)
                .Select(item => CategoryResponse.From(item))
                .ToListAsync(cancellationToken));

        group.MapPost("/", async (CategoryRequest request, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var validationErrors = AdminValidationPolicy.ValidateCategory(request);
            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var category = new Category { Title = request.Title.Trim(), Image = request.Image };
            db.Categories.Add(category);
            await db.SaveChangesAsync(cancellationToken);
            return Results.Created($"/api/admin/categories/{category.Id}", CategoryResponse.From(category));
        });

        group.MapPut("/{id:int}", async (int id, CategoryRequest request, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var validationErrors = AdminValidationPolicy.ValidateCategory(request);
            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var category = await db.Categories.FindAsync([id], cancellationToken);
            if (category is null)
            {
                return Results.NotFound();
            }

            category.Title = request.Title.Trim();
            category.Image = request.Image;
            await db.SaveChangesAsync(cancellationToken);
            return Results.Ok(CategoryResponse.From(category));
        });

        group.MapDelete("/{id:int}", async (int id, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var category = await db.Categories.Include(item => item.SubCategories).FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
            if (category is null)
            {
                return Results.NotFound();
            }

            var subCategoryIds = category.SubCategories.Select(item => item.Id).ToArray();
            await db.Products.Where(product => product.SubCategoryId != null && subCategoryIds.Contains(product.SubCategoryId.Value))
                .ExecuteUpdateAsync(setters => setters.SetProperty(product => product.SubCategoryId, (int?)null), cancellationToken);
            db.Categories.Remove(category);
            await db.SaveChangesAsync(cancellationToken);
            return Results.NoContent();
        });
    }

    private static void MapSubCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/subcategories").RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole));

        group.MapGet("/", async (int? categoryId, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var query = db.SubCategories.Include(item => item.Category).AsQueryable();
            if (categoryId is not null)
            {
                query = query.Where(item => item.CategoryId == categoryId);
            }

            return await query
                .OrderBy(item => item.Title)
                .Select(item => SubCategoryResponse.From(item))
                .ToListAsync(cancellationToken);
        });

        group.MapPost("/", async (SubCategoryRequest request, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var validationErrors = AdminValidationPolicy.ValidateSubCategory(request);
            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var subCategory = new SubCategory { CategoryId = request.CategoryId, Title = request.Title.Trim() };
            db.SubCategories.Add(subCategory);
            await db.SaveChangesAsync(cancellationToken);
            return Results.Created($"/api/admin/subcategories/{subCategory.Id}", SubCategoryResponse.From(subCategory));
        });

        group.MapPut("/{id:int}", async (int id, SubCategoryRequest request, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var validationErrors = AdminValidationPolicy.ValidateSubCategory(request);
            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var subCategory = await db.SubCategories.FindAsync([id], cancellationToken);
            if (subCategory is null)
            {
                return Results.NotFound();
            }

            subCategory.CategoryId = request.CategoryId;
            subCategory.Title = request.Title.Trim();
            await db.SaveChangesAsync(cancellationToken);
            return Results.Ok(SubCategoryResponse.From(subCategory));
        });

        group.MapDelete("/{id:int}", async (int id, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var subCategory = await db.SubCategories.FindAsync([id], cancellationToken);
            if (subCategory is null)
            {
                return Results.NotFound();
            }

            await db.Products.Where(product => product.SubCategoryId == id)
                .ExecuteUpdateAsync(setters => setters.SetProperty(product => product.SubCategoryId, (int?)null), cancellationToken);
            db.SubCategories.Remove(subCategory);
            await db.SaveChangesAsync(cancellationToken);
            return Results.NoContent();
        });
    }

    private static void MapAttributeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/attributes").RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole));

        group.MapGet("/", async (MarketplaceDbContext db, CancellationToken cancellationToken) =>
            await db.Attributes
                .OrderBy(item => item.Name)
                .Select(item => new AttributeResponse(item.Id, item.Name))
                .ToListAsync(cancellationToken));

        group.MapPost("/", async (AttributeRequest request, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var validationErrors = AdminValidationPolicy.ValidateAttribute(request);
            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var attribute = new AttributeDefinition { Name = request.Name.Trim() };
            db.Attributes.Add(attribute);
            await db.SaveChangesAsync(cancellationToken);
            return Results.Created($"/api/admin/attributes/{attribute.Id}", new AttributeResponse(attribute.Id, attribute.Name));
        });

        group.MapPut("/{id:int}", async (int id, AttributeRequest request, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var validationErrors = AdminValidationPolicy.ValidateAttribute(request);
            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var attribute = await db.Attributes.FindAsync([id], cancellationToken);
            if (attribute is null)
            {
                return Results.NotFound();
            }

            attribute.Name = request.Name.Trim();
            await db.SaveChangesAsync(cancellationToken);
            return Results.Ok(new AttributeResponse(attribute.Id, attribute.Name));
        });

        group.MapDelete("/{id:int}", async (int id, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var attribute = await db.Attributes.FindAsync([id], cancellationToken);
            if (attribute is null)
            {
                return Results.NotFound();
            }

            db.Attributes.Remove(attribute);
            await db.SaveChangesAsync(cancellationToken);
            return Results.NoContent();
        });
    }

    private static void MapSellerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/sellers").RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole, MarketplaceSeed.SellerRole));

        group.MapGet("/", async (MarketplaceDbContext db, CancellationToken cancellationToken) =>
            await db.Sellers
                .Include(item => item.User)
                .OrderBy(item => item.User!.Name)
                .Select(item => SellerResponse.From(item))
                .ToListAsync(cancellationToken));

        group.MapGet("/{id:guid}", async (Guid id, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var seller = await db.Sellers.Include(item => item.User).FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
            return seller is null ? Results.NotFound() : Results.Ok(SellerResponse.From(seller));
        });

        group.MapPost("/", async (SellerCreateRequest request, UserManager<ApplicationUser> userManager, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var validationErrors = AdminValidationPolicy.ValidateSeller(request, passwordRequired: true);
            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors);
            }

            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            var user = new ApplicationUser
            {
                UserName = request.Login.Trim(),
                Email = $"{request.Login.Trim()}@marketplace.local",
                Name = request.Name.Trim(),
                LastName = request.LastName.Trim(),
                Cpf = AdminValidationPolicy.NormalizeDocument(request.Cpf),
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, request.Password!);
            if (!result.Succeeded)
            {
                return Results.ValidationProblem(result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description }));
            }

            await userManager.AddToRoleAsync(user, MarketplaceSeed.SellerRole);
            var seller = new Seller
            {
                UserId = user.Id,
                Age = request.Age,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth,
                Website = request.Website,
                Company = request.Company,
                Cnpj = request.Cnpj,
                BranchOfActivity = request.BranchOfActivity,
                FantasyName = request.FantasyName
            };

            db.Sellers.Add(seller);
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            seller.User = user;
            return Results.Created($"/api/admin/sellers/{seller.Id}", SellerResponse.From(seller));
        }).RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole));

        group.MapPut("/{id:guid}", async (Guid id, SellerRequest request, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var validationErrors = AdminValidationPolicy.ValidateSeller(request, passwordRequired: false);
            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var seller = await db.Sellers.Include(item => item.User).FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
            if (seller is null)
            {
                return Results.NotFound();
            }

            seller.Email = request.Email;
            seller.Website = request.Website;
            seller.Company = request.Company;
            seller.Cnpj = request.Cnpj;
            seller.BranchOfActivity = request.BranchOfActivity;
            seller.FantasyName = request.FantasyName;
            seller.Age = request.Age;
            seller.DateOfBirth = request.DateOfBirth;
            if (seller.User is not null)
            {
                seller.User.Name = request.Name;
                seller.User.LastName = request.LastName;
                seller.User.Cpf = AdminValidationPolicy.NormalizeDocument(request.Cpf);
            }

            await db.SaveChangesAsync(cancellationToken);
            return Results.Ok(SellerResponse.From(seller));
        });

        group.MapDelete("/{id:guid}", async (Guid id, UserManager<ApplicationUser> userManager, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var seller = await db.Sellers.Include(item => item.User).FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
            if (seller is null)
            {
                return Results.NotFound();
            }

            db.Sellers.Remove(seller);
            await db.SaveChangesAsync(cancellationToken);

            if (seller.User is not null)
            {
                await userManager.RemoveFromRoleAsync(seller.User, MarketplaceSeed.SellerRole);
                if (!await userManager.IsInRoleAsync(seller.User, MarketplaceSeed.CustomerRole))
                {
                    await userManager.AddToRoleAsync(seller.User, MarketplaceSeed.CustomerRole);
                }
            }

            return Results.NoContent();
        }).RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole));
    }

    private static void MapCarouselEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/carousel").RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole));

        group.MapGet("/", async (MarketplaceDbContext db, CancellationToken cancellationToken) =>
            await db.CarouselImages
                .OrderBy(item => item.SortOrder)
                .Select(item => new CarouselResponse(item.Id, item.FileName, item.SortOrder))
                .ToListAsync(cancellationToken));

        group.MapPost("/", async (CarouselRequest request, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.FileName))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["fileName"] = ["Imagem obrigatoria."] });
            }

            var image = new CarouselImage
            {
                FileName = request.FileName.Trim(),
                SortOrder = request.SortOrder
            };
            db.CarouselImages.Add(image);
            await db.SaveChangesAsync(cancellationToken);
            return Results.Created($"/api/admin/carousel/{image.Id}", new CarouselResponse(image.Id, image.FileName, image.SortOrder));
        });

        group.MapPut("/{id:int}", async (int id, CarouselRequest request, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var image = await db.CarouselImages.FindAsync([id], cancellationToken);
            if (image is null)
            {
                return Results.NotFound();
            }

            image.FileName = request.FileName.Trim();
            image.SortOrder = request.SortOrder;
            await db.SaveChangesAsync(cancellationToken);
            return Results.Ok(new CarouselResponse(image.Id, image.FileName, image.SortOrder));
        });

        group.MapDelete("/{id:int}", async (int id, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var image = await db.CarouselImages.FindAsync([id], cancellationToken);
            if (image is null)
            {
                return Results.NotFound();
            }

            db.CarouselImages.Remove(image);
            await db.SaveChangesAsync(cancellationToken);
            return Results.NoContent();
        });
    }

    private static void MapUploadEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/admin/uploads/{scope}", async (string scope, IFormFile file, IWebHostEnvironment environment, CancellationToken cancellationToken) =>
        {
            if (file.Length == 0)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["file"] = ["Arquivo obrigatorio."] });
            }

            if (file.Length > UploadPolicy.MaxSizeBytes)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["file"] = ["Imagem deve ter no maximo 5 MB."] });
            }

            if (!UploadPolicy.IsSupportedImage(file.FileName, file.ContentType))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["file"] = ["Formato de imagem invalido."] });
            }

            scope = UploadPolicy.NormalizeScope(scope);
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid():N}{extension}";
            var relativeDirectory = Path.Combine("uploads", scope);
            var absoluteDirectory = Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), relativeDirectory);
            Directory.CreateDirectory(absoluteDirectory);

            var absolutePath = Path.Combine(absoluteDirectory, fileName);
            await using var stream = File.Create(absolutePath);
            await file.CopyToAsync(stream, cancellationToken);

            var publicPath = $"/uploads/{scope}/{fileName}";
            return Results.Ok(new UploadResponse(fileName, publicPath));
        })
        .RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole, MarketplaceSeed.SellerRole))
        .DisableAntiforgery();
    }

    private static string NormalizeRole(string? role) =>
        role is MarketplaceSeed.AdminRole or MarketplaceSeed.SellerRole or MarketplaceSeed.CustomerRole
            ? role
            : MarketplaceSeed.CustomerRole;
}

public static class UploadPolicy
{
    public const long MaxSizeBytes = 5 * 1024 * 1024;

    public static bool IsSupportedImage(string fileName, string contentType)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return !string.IsNullOrWhiteSpace(fileName)
            && (extension is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp")
            && contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }

    public static string NormalizeScope(string scope) =>
        scope.Trim().ToLowerInvariant() switch
        {
            "categories" or "category" or "categorias" => "categories",
            "carousel" or "destaques" => "carousel",
            _ => "products"
        };
}

public static class AdminValidationPolicy
{
    public static Dictionary<string, string[]> ValidateUser(UserRequest request, bool passwordRequired)
    {
        var errors = new Dictionary<string, string[]>();
        AddRequired(errors, "name", request.Name, "Nome obrigatorio.");
        AddRequired(errors, "lastName", request.LastName, "Sobrenome obrigatorio.");
        AddRequired(errors, "login", request.Login, "Login obrigatorio.");

        if (passwordRequired)
        {
            AddRequired(errors, "password", request.Password, "Senha obrigatoria.");
        }

        ValidateDocument(errors, request.Cpf, "cpf");
        return errors;
    }

    public static Dictionary<string, string[]> ValidateSeller(SellerCreateRequest request, bool passwordRequired)
    {
        var errors = new Dictionary<string, string[]>();
        AddRequired(errors, "name", request.Name, "Nome obrigatorio.");
        AddRequired(errors, "lastName", request.LastName, "Sobrenome obrigatorio.");
        AddRequired(errors, "login", request.Login, "Login obrigatorio.");

        if (passwordRequired)
        {
            AddRequired(errors, "password", request.Password, "Senha obrigatoria.");
        }

        ValidateDocument(errors, request.Cpf, "cpf");
        return errors;
    }

    public static Dictionary<string, string[]> ValidateSeller(SellerRequest request, bool passwordRequired)
    {
        var errors = new Dictionary<string, string[]>();
        AddRequired(errors, "name", request.Name, "Nome obrigatorio.");
        AddRequired(errors, "lastName", request.LastName, "Sobrenome obrigatorio.");
        ValidateDocument(errors, request.Cpf, "cpf");
        return errors;
    }

    public static Dictionary<string, string[]> ValidateCategory(CategoryRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        AddRequired(errors, "title", request.Title, "Titulo obrigatorio.");
        return errors;
    }

    public static Dictionary<string, string[]> ValidateSubCategory(SubCategoryRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        AddRequired(errors, "title", request.Title, "Titulo obrigatorio.");
        if (request.CategoryId <= 0)
        {
            errors["categoryId"] = ["Categoria obrigatoria."];
        }

        return errors;
    }

    public static Dictionary<string, string[]> ValidateAttribute(AttributeRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        AddRequired(errors, "name", request.Name, "Nome obrigatorio.");
        return errors;
    }

    public static string? NormalizeDocument(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var digits = new string(value.Where(char.IsDigit).ToArray());
        return string.IsNullOrWhiteSpace(digits) ? null : digits;
    }

    private static void AddRequired(Dictionary<string, string[]> errors, string key, string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors[key] = [message];
        }
    }

    private static void ValidateDocument(Dictionary<string, string[]> errors, string? value, string key)
    {
        var normalized = NormalizeDocument(value);
        if (!string.IsNullOrWhiteSpace(normalized) && normalized.Length != 11)
        {
            errors[key] = ["CPF deve conter 11 digitos."];
        }
    }
}

public sealed record UserResponse(Guid Id, string Login, string Name, string LastName, string? Cpf, string Role)
{
    public static UserResponse From(ApplicationUser user, string[] roles) =>
        new(user.Id, user.UserName ?? string.Empty, user.Name, user.LastName, user.Cpf, roles.FirstOrDefault() ?? MarketplaceSeed.CustomerRole);
}

public sealed record UserRequest(string Name, string LastName, string Login, string? Password, string? Cpf, string Role);
public sealed record ResetPasswordRequest(string Password);

public sealed record SellerResponse(
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
    string? FantasyName)
{
    public static SellerResponse From(Seller seller) =>
        new(
            seller.Id,
            seller.UserId,
            seller.User?.Name ?? string.Empty,
            seller.User?.LastName ?? string.Empty,
            seller.User?.Cpf,
            seller.Age,
            seller.Email,
            seller.DateOfBirth,
            seller.Website,
            seller.Company,
            seller.Cnpj,
            seller.BranchOfActivity,
            seller.FantasyName);
}

public sealed record CategoryResponse(int Id, string Title, string? Image, SubCategoryOptionResponse[] SubCategories)
{
    public static CategoryResponse From(Category category) =>
        new(
            category.Id,
            category.Title,
            category.Image,
            category.SubCategories
                .OrderBy(subCategory => subCategory.Title)
                .Select(subCategory => new SubCategoryOptionResponse(subCategory.Id, subCategory.Title))
                .ToArray());
}

public sealed record SubCategoryResponse(int Id, int CategoryId, string Title, string Category)
{
    public static SubCategoryResponse From(SubCategory subCategory) =>
        new(subCategory.Id, subCategory.CategoryId, subCategory.Title, subCategory.Category?.Title ?? string.Empty);
}

public sealed record SubCategoryOptionResponse(int Id, string Title);
public sealed record AttributeResponse(int Id, string Name);
public sealed record CategoryRequest(string Title, string? Image);
public sealed record SubCategoryRequest(int CategoryId, string Title);
public sealed record AttributeRequest(string Name);
public sealed record CarouselResponse(int Id, string FileName, int SortOrder);
public sealed record CarouselRequest(string FileName, int SortOrder);
public sealed record UploadResponse(string FileName, string Url);

public static class AdminPasswordResetPolicy
{
    public const string RequiredRole = MarketplaceSeed.AdminRole;

    public static Dictionary<string, string[]> Validate(ResetPasswordRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors["password"] = ["Senha obrigatoria."];
        }

        return errors;
    }
}

public sealed record SellerRequest(
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
public sealed record SellerCreateRequest(
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
