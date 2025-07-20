using Claims.Auditing.BackgroundProcessing;

namespace Claims.Auditing;

public class Auditer(IAuditQueue auditQueue) : IAuditer
{
    public void AuditClaim(string id, string httpRequestType)
    {
        Audit<ClaimAuditEntity>(id, httpRequestType);
    }

    public void AuditCover(string id, string httpRequestType)
    {
        Audit<CoverAuditEntity>(id, httpRequestType);
    }

    private void Audit<T>(string id, string httpRequestType) where T : BaseAuditEntity, new()
    {
        var entity = new T
        {
            Created = DateTime.Now,
            HttpRequestType = httpRequestType
        };

        switch (entity)
        {
            case ClaimAuditEntity claimEntity:
                claimEntity.ClaimId = id;
                break;
            case CoverAuditEntity coverEntity:
                coverEntity.CoverId = id;
                break;
        }

        auditQueue.Enqueue(entity);
    }
}