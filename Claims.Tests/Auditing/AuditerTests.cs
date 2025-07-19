using Claims.Auditing;
using Claims.Auditing.BackgroundProcessing;
using Moq;
using Xunit;

namespace Claims.Tests.Auditing;

public class AuditerTests
{
    private readonly Mock<IAuditQueue> _mockAuditQueue = new();
    private readonly Auditer _auditer;

    public AuditerTests()
    {
        _auditer = new Auditer(_mockAuditQueue.Object);
    }

    [Fact]
    public void AuditClaim_ValidClaimId_EnqueuesClaimAuditEntity()
    {
        // Arrange
        var claimId = "claim-123";
        var httpRequestType = "POST";

        // Act
        _auditer.AuditClaim(claimId, httpRequestType);

        // Assert
        _mockAuditQueue.Verify(x => x.Enqueue(It.Is<ClaimAuditEntity>(entity =>
            entity.ClaimId == claimId &&
            entity.HttpRequestType == httpRequestType &&
            entity.Created != default)), Times.Once);
    }

    [Fact]
    public void AuditCover_ValidCoverId_EnqueuesCoverAuditEntity()
    {
        // Arrange
        var coverId = "cover-456";
        var httpRequestType = "DELETE";

        // Act
        _auditer.AuditCover(coverId, httpRequestType);

        // Assert
        _mockAuditQueue.Verify(x => x.Enqueue(It.Is<CoverAuditEntity>(entity =>
            entity.CoverId == coverId &&
            entity.HttpRequestType == httpRequestType &&
            entity.Created != default)), Times.Once);
    }
}
