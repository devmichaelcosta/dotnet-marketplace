using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Marketplace.Api.Features.Admin.Produto.ProductImports.Shared;

public sealed class ProductImportQueue
{
    private readonly Channel<int> channel = Channel.CreateUnbounded<int>();
    private readonly ConcurrentDictionary<int, byte> queued = [];

    public async ValueTask EnqueueAsync(int jobId, CancellationToken cancellationToken = default)
    {
        if (!queued.TryAdd(jobId, 0))
        {
            return;
        }

        await channel.Writer.WriteAsync(jobId, cancellationToken);
    }

    public async ValueTask<int> DequeueAsync(CancellationToken cancellationToken)
    {
        var jobId = await channel.Reader.ReadAsync(cancellationToken);
        queued.TryRemove(jobId, out _);
        return jobId;
    }
}

