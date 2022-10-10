using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Infra.Kafka;

partial class KafkaHostedService<TKey, TValue, TSerializer>
{
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (backgroundTask is null)
        {
            return;
        }

        try
        {
            cancellationTokenSource?.Cancel();
        }
        finally
        {
            await Task.WhenAny(backgroundTask, Task.Delay(Timeout.Infinite, cancellationToken)).ConfigureAwait(false);
        }
    }
}