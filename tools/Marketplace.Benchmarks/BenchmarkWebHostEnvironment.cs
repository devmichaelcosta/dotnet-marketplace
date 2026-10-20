using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

public sealed class BenchmarkWebHostEnvironment : IWebHostEnvironment
{
    public BenchmarkWebHostEnvironment(string contentRoot, string webRoot)
    {
        ContentRootPath = contentRoot;
        WebRootPath = webRoot;
        Directory.CreateDirectory(ContentRootPath);
        Directory.CreateDirectory(WebRootPath);
        ContentRootFileProvider = new PhysicalFileProvider(ContentRootPath);
        WebRootFileProvider = new PhysicalFileProvider(WebRootPath);
    }

    public string ApplicationName { get; set; } = "Marketplace.Benchmarks";
    public IFileProvider WebRootFileProvider { get; set; }
    public string WebRootPath { get; set; }
    public string EnvironmentName { get; set; } = Environments.Production;
    public string ContentRootPath { get; set; }
    public IFileProvider ContentRootFileProvider { get; set; }
}
