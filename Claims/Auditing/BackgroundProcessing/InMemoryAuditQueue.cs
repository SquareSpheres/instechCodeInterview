using System.Threading.Channels;

namespace Claims.Auditing.BackgroundProcessing;

public class InMemoryAuditQueue(ILogger<InMemoryAuditQueue> logger) : IAuditQueue
{
    private readonly Channel<BaseAuditEntity> _channel = Channel.CreateUnbounded<BaseAuditEntity>(
        new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

    public void Enqueue<T>(T item) where T : BaseAuditEntity
    {
        try
        {
            if (!_channel.Writer.TryWrite(item))
                logger.LogError("Failed to enqueue audit item of type {ItemType}", typeof(T).Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error enqueueing audit item of type {ItemType}", typeof(T).Name);
        }
    }

    public async Task<BaseAuditEntity?> DequeueAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _channel.Reader.ReadAsync(cancellationToken);
        }
        catch (ChannelClosedException)
        {
            logger.LogInformation("Audit queue channel closed");
            return null;
        }
        catch (OperationCanceledException)
        {
            logger.LogDebug("Dequeue operation cancelled");
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error dequeuing audit item");
            return null;
        }
    }
}