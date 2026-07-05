using Marketplace.Web.Components;
using Marketplace.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<ClientState>();
builder.Services.AddScoped<MarketplaceApiClient>();
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

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
