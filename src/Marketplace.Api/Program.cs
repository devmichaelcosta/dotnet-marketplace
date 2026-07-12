using System.Text;
using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin;
using Marketplace.Api.Features.Auth;
using Marketplace.Api.Features.Cart;
using Marketplace.Api.Features.Catalog;
using Marketplace.Api.Features.Orders;
using Marketplace.Api.Features.ProductImports;
using Marketplace.Api.Features.Products;
using Marketplace.Api.Features.Products.Admin;
using Marketplace.Api.Features.Users;
using Marketplace.Api.Infrastructure.Persistence;
using Marketplace.Api.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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

var jwtKey = builder.Configuration["Jwt:Key"] ?? "development-only-marketplace-secret-key-32";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<TokenService>();
builder.Services.AddHttpClient("product-import-images", client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("DotNetMarketplaceProductImport/1.0");
});
builder.Services.AddSingleton<ProductImportQueue>();
builder.Services.AddScoped<ProductImportProcessor>();
builder.Services.AddScoped<ProductImportImageDownloader>();
builder.Services.AddHostedService<ProductImportWorker>();
builder.Services.AddProductAdminModule();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(_ => true));
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", () => Results.Ok(new { Status = "Healthy", Service = "Marketplace.Api" }));
app.MapAuthEndpoints();
app.MapCatalogEndpoints();
app.MapAdminEndpoints();
app.MapProductAdminEndpoints();
app.MapProductEndpoints();
app.MapProductImportEndpoints();
app.MapCartEndpoints();
app.MapOrderEndpoints();
app.MapUserEndpoints();

if (app.Environment.IsDevelopment() && app.Configuration.GetValue("SeedDatabase", false))
{
    await MarketplaceSeed.SeedAsync(app.Services);
}

app.Run();

public partial class Program;
