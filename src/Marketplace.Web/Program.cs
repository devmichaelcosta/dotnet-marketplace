using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Components.Authorization;
using Marketplace.Web.Components;
using Marketplace.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();
var dataProtectionKeyDirectory = builder.Configuration["DataProtection:KeyDirectory"]
    ?? Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "DotNetMarketplace",
        "DataProtection");
Directory.CreateDirectory(dataProtectionKeyDirectory);
builder.Services.AddDataProtection()
    .SetApplicationName("DotNetMarketplace")
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeyDirectory));

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
        var cookieDomain = builder.Configuration["Authentication:CookieDomain"];
        if (!string.IsNullOrWhiteSpace(cookieDomain))
        {
            options.Cookie.Domain = cookieDomain;
        }
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

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
