using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Marketplace.Web.Components;
using Marketplace.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Marketplace.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddScoped<ClientState>();
builder.Services.AddScoped<MarketplaceApiClient>();
builder.Services.AddHttpClient("marketplace-api", (provider, client) =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(configuration["ApiBaseUrl"] ?? "http://localhost:5000/");
});
builder.Services.AddHttpClient<MarketplaceApiClient>((provider, client) =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(configuration["ApiBaseUrl"] ?? "http://localhost:5000/");
});
builder.Services.AddHttpClient<MarketplaceAuthService>((provider, client) =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(configuration["ApiBaseUrl"] ?? "http://localhost:5000/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/auth/antiforgery", (IAntiforgery antiforgery, HttpContext http) =>
{
    var tokens = antiforgery.GetAndStoreTokens(http);
    return Results.Ok(new WebAntiforgeryToken(tokens.RequestToken ?? string.Empty));
});

app.MapPost("/auth/login", async (
    WebLoginRequest request,
    MarketplaceAuthService authService,
    IAntiforgery antiforgery,
    HttpContext http,
    CancellationToken cancellationToken) =>
{
    try
    {
        await antiforgery.ValidateRequestAsync(http);
    }
    catch (AntiforgeryValidationException)
    {
        return Results.BadRequest(new WebLoginResult(false, "Sessao expirada. Atualize a pagina e tente novamente."));
    }

    if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new WebLoginResult(false, "Informe login e senha."));
    }

    ClaimsPrincipal? principal;
    try
    {
        principal = await authService.AuthenticateAsync(request, cancellationToken);
    }
    catch (HttpRequestException)
    {
        return Results.Problem("Servico de autenticacao indisponivel.", statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    if (principal is null)
    {
        return Results.Unauthorized();
    }

    await http.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal,
        new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
            AllowRefresh = false
        });

    return Results.Ok(new WebLoginResult(true, null));
});

app.MapPost("/auth/logout", async (IAntiforgery antiforgery, HttpContext http) =>
{
    try
    {
        await antiforgery.ValidateRequestAsync(http);
    }
    catch (AntiforgeryValidationException)
    {
        return Results.BadRequest(new WebLoginResult(false, "Sessao expirada. Atualize a pagina e tente novamente."));
    }

    await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok(new WebLoginResult(true, null));
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public sealed record WebAntiforgeryToken(string Token);
