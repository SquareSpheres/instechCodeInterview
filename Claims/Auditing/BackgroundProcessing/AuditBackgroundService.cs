namespace Claims.Auditing.BackgroundProcessing;

public class AuditBackgroundService(
    IAuditQueue auditQueue,
    IServiceProvider serviceProvider,
    ILogger<BackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("AuditBackgroundService is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var auditEntity = await auditQueue.DequeueAsync(stoppingToken);

                if (auditEntity != null)
                {
                    await ProcessAuditAsync(auditEntity, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred processing audit entity.");
            }
        }

        logger.LogInformation("AuditBackgroundService is stopping.");
    }

    private async Task ProcessAuditAsync(BaseAuditEntity auditEntity, CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var auditContext = scope.ServiceProvider.GetRequiredService<AuditContext>();

        try
        {
            auditContext.Add(auditEntity);
            await auditContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Processed audit entity: {EntityType}", auditEntity.GetType().Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process audit entity: {EntityType}", auditEntity.GetType().Name);
        }
    }
}