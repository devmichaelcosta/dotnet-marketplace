using System.Net;
using System.Net.Http;

public sealed class BenchmarkHttpClientFactory(byte[] imageBytes) : IHttpClientFactory
{
    public HttpClient CreateClient(string name) => new(new StaticImageHandler(imageBytes), disposeHandler: true);
}
