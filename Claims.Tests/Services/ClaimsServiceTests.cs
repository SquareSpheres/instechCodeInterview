using Claims.Auditing;
using Claims.Core.Infrastructure;
using Claims.Features.Claims.Models;
using Claims.Features.Claims.Repositories;
using Claims.Features.Claims.Services;
using Claims.Features.Covers.Models;
using Claims.Features.Covers.Repositories;
using Moq;
using Xunit;

namespace Claims.Tests.Services;

public class ClaimsServiceTests
{
    private readonly Mock<IAuditer> _mockAuditer = new();
    private readonly Mock<IClaimsRepository> _mockClaimsRepository = new();
    private readonly Mock<ICoverRepository> _mockCoverRepository = new();
    private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
    private readonly ClaimsService _service;

    public ClaimsServiceTests()
    {
        _service = new ClaimsService(
            _mockAuditer.Object,
            _mockClaimsRepository.Object,
            _mockCoverRepository.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidClaim_CreatesClaimSuccessfully()
    {
        // Arrange
        var coverEntity = new CoverEntity
        {
            Id = "cover-1",
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
            Type = CoverType.Yacht,
            Premium = 1000m
        };

        var createClaimDto = new CreateClaimDto
        {
            CoverId = "cover-1",
            Name = "Test Claim",
            Type = ClaimType.Collision,
            DamageCost = 5000m
        };

        _mockCoverRepository.Setup(x => x.GetCoverOrNullAsync("cover-1"))
            .ReturnsAsync(coverEntity);

        // Act
        var result = await _service.CreateAsync(createClaimDto);

        // Assert
        Assert.Equal(createClaimDto.CoverId, result.CoverId);
        Assert.Equal(createClaimDto.Name, result.Name);
        Assert.Equal(createClaimDto.Type, result.Type);
        Assert.Equal(createClaimDto.DamageCost, result.DamageCost);
        
        _mockClaimsRepository.Verify(x => x.AddItemAsync(It.IsAny<ClaimEntity>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(default), Times.Once);
        _mockAuditer.Verify(x => x.AuditClaim(It.IsAny<string>(), "POST"), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ClaimOutsideCoveragePeriod_ThrowsArgumentException()
    {
        // Arrange
        var coverEntity = new CoverEntity
        {
            Id = "cover-1",
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10)), // Future start date
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(20)),
            Type = CoverType.Yacht,
            Premium = 1000m
        };

        var createClaimDto = new CreateClaimDto
        {
            CoverId = "cover-1",
            Name = "Test Claim",
            Type = ClaimType.Collision,
            DamageCost = 5000m
        };

        _mockCoverRepository.Setup(x => x.GetCoverOrNullAsync("cover-1"))
            .ReturnsAsync(coverEntity);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createClaimDto));
        Assert.Contains("Claims can only be made during the coverage period", exception.Message);
    }
}
