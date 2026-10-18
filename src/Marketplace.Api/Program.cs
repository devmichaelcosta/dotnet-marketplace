using System.Threading.RateLimiting;
using Marketplace.Api.Domain;
using Marketplace.Api.Shared.Admin;
using Marketplace.Api.Modules.Orders.Admin;
using Marketplace.Api.Modules.Produto.Admin;
using Marketplace.Api.Modules.ProductImports.Admin;
using Marketplace.Api.Modules.Ratings.Admin;
using Marketplace.Api.Modules.Ratings.Website;
using Marketplace.Api.Modules.Auth.Website;
using Marketplace.Api.Modules.Auth.Website.Shared;
using Marketplace.Api.Modules.Cart.Website;
using Marketplace.Api.Modules.Catalog.Website;
using Marketplace.Api.Modules.Orders.Website;
using Marketplace.Api.Modules.Produto.Website;
using Marketplace.Api.Modules.Carousel.Website;
using Marketplace.Api.Modules.Users.Website;
using Marketplace.Api.Infrastructure.Persistence;
using Marketplace.Api.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddDbContext<MarketplaceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Marketplace")));

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<MarketplaceDbContext>()
    .AddSignInManager();

var jwtKey = JwtSigningKey.Create(builder.Configuration["Jwt:Key"]);
var authenticationBuilder = builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "Marketplace",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "Marketplace",
            IssuerSigningKey = jwtKey
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<TokenService>();
builder.Services.Configure<PasswordResetOptions>(builder.Configuration.GetSection(PasswordResetOptions.SectionName));
builder.Services.Configure<SmtpEmailOptions>(builder.Configuration.GetSection(SmtpEmailOptions.SectionName));
builder.Services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(1));

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("password-reset-request", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            AutoReplenishment = true,
            PermitLimit = 5,
            QueueLimit = 0,
            Window = TimeSpan.FromMinutes(15)
        }));
    options.AddPolicy("password-reset-confirm", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            AutoReplenishment = true,
            PermitLimit = 10,
            QueueLimit = 0,
            Window = TimeSpan.FromMinutes(15)
        }));
});

var dataProtectionKeyDirectory = builder.Configuration["DataProtection:KeyDirectory"]
    ?? Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "DotNetMarketplace",
        "DataProtection");
Directory.CreateDirectory(dataProtectionKeyDirectory);
builder.Services.AddDataProtection()
    .SetApplicationName("DotNetMarketplace")
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeyDirectory));

builder.Services.AddAntiforgery();
builder.Services.AddAdminModule();
builder.Services.AddAdminOrdersModule();
builder.Services.AddProdutoAdminModule();
builder.Services.AddProductImportsAdminModule();
builder.Services.AddProdutoWebsiteModule();
builder.Services.AddRatingsAdminModule();
builder.Services.AddRatingsWebsiteModule();
builder.Services.AddWebsiteAuthModule();
builder.Services.AddWebsiteCartModule();
builder.Services.AddWebsiteCatalogModule();
builder.Services.AddWebsiteCarouselModule();
builder.Services.AddWebsiteOrdersModule();
builder.Services.AddWebsiteUsersModule();
authenticationBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
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
var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors();
app.UseRateLimiter();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapGet("/api/health", () => Results.Ok(new { Status = "Healthy", Service = "Marketplace.Api" }));
app.MapAdminEndpoints();
app.MapAdminOrdersEndpoints();
app.MapProdutoAdminEndpoints();
app.MapProdutoWebsiteEndpoints();
app.MapRatingsAdminEndpoints();
app.MapRatingsWebsiteEndpoints();
app.MapProductImportsAdminEndpoints();
app.MapWebsiteAuthEndpoints();
app.MapWebsiteCatalogEndpoints();
app.MapWebsiteCartEndpoints();
app.MapWebsiteOrdersEndpoints();
app.MapWebsiteUserEndpoints();

if (app.Environment.IsDevelopment() && app.Configuration.GetValue("SeedDatabase", false))
{
    await MarketplaceSeed.SeedAsync(app.Services);
}

app.Run();

public partial class Program;



