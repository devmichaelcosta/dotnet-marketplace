using Marketplace.Api.Domain;
using Marketplace.Api.Infrastructure.Persistence;
using Marketplace.Api.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", async (
            RegisterRequest request,
            UserManager<ApplicationUser> userManager,
            CancellationToken cancellationToken) =>
        {
            var validation = ValidateRegistration(request);
            if (validation is not null)
            {
                return validation;
            }

            var user = new ApplicationUser
            {
                UserName = request.Login.Trim(),
                Email = $"{request.Login.Trim()}@marketplace.local",
                Name = request.Name.Trim(),
                LastName = request.LastName.Trim(),
                Cpf = NormalizeDocument(request.Cpf),
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return Results.ValidationProblem(result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description }));
            }

            await userManager.AddToRoleAsync(user, MarketplaceSeed.CustomerRole);
            return Results.Created($"/api/users/{user.Id}", new { user.Id, user.UserName, user.Name });
        });

        group.MapPost("/register-seller", async (
            RegisterRequest request,
            UserManager<ApplicationUser> userManager,
            MarketplaceDbContext db,
            CancellationToken cancellationToken) =>
        {
            var validation = ValidateRegistration(request);
            if (validation is not null)
            {
                return validation;
            }

            var user = new ApplicationUser
            {
                UserName = request.Login.Trim(),
                Email = $"{request.Login.Trim()}@marketplace.local",
                Name = request.Name.Trim(),
                LastName = request.LastName.Trim(),
                Cpf = NormalizeDocument(request.Cpf),
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return Results.ValidationProblem(result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description }));
            }

            await userManager.AddToRoleAsync(user, MarketplaceSeed.SellerRole);
            db.Sellers.Add(new Seller { UserId = user.Id });
            await db.SaveChangesAsync(cancellationToken);

            return Results.Created($"/api/sellers/{user.Id}", new { user.Id, user.UserName, user.Name });
        });

        group.MapPost("/login", async (
            LoginRequest request,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            TokenService tokenService) =>
        {
            var user = await userManager.Users.FirstOrDefaultAsync(item => item.UserName == request.Login);
            if (user is null)
            {
                return Results.Unauthorized();
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return Results.Unauthorized();
            }

            var roles = await userManager.GetRolesAsync(user);
            var token = await tokenService.CreateTokenAsync(user);
            return Results.Ok(new { Token = token, User = new { user.Id, user.UserName, user.Name, Roles = roles } });
        });

        return app;
    }

    private static IResult? ValidateRegistration(RegisterRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors["name"] = ["Nome obrigatorio."];
        }

        if (string.IsNullOrWhiteSpace(request.Login) || request.Login.Trim().Length < 3)
        {
            errors["login"] = ["Login deve ter pelo menos 3 caracteres."];
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
        {
            errors["password"] = ["Senha deve ter pelo menos 6 caracteres."];
        }

        var cpf = NormalizeDocument(request.Cpf);
        if (!string.IsNullOrWhiteSpace(cpf) && cpf.Length != 11)
        {
            errors["cpf"] = ["CPF deve conter 11 digitos."];
        }

        return errors.Count == 0 ? null : Results.ValidationProblem(errors);
    }

    private static string? NormalizeDocument(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var digits = new string(value.Where(char.IsDigit).ToArray());
        return string.IsNullOrWhiteSpace(digits) ? null : digits;
    }
}

public sealed record RegisterRequest(string Name, string LastName, string Login, string Password, string? Cpf);
public sealed record LoginRequest(string Login, string Password);
