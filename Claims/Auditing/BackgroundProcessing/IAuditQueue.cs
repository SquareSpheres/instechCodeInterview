using System.Diagnostics.CodeAnalysis;

namespace Claims.Auditing.BackgroundProcessing;

public interface IAuditQueue
{
    void Enqueue<T>(T item) where T : BaseAuditEntity;
    Task<BaseAuditEntity?> DequeueAsync(CancellationToken cancellationToken);
}
