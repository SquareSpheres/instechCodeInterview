using Claims.Auditing;
using Claims.Core.Infrastructure;
using Claims.Features.Covers.Models;
using Claims.Features.Covers.Repositories;
using Claims.Features.Covers.Services;
using Moq;
using Xunit;

namespace Claims.Tests.Services;

public class CoverServiceTests
{
    private readonly Mock<IAuditer> _mockAuditer = new();
    private readonly Mock<ICoverRepository> _mockCoverRepository = new();
    private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
    private readonly Mock<IPremiumCalculatorService> _mockPremiumCalculatorService = new();
    private readonly CoverService _service;

    public CoverServiceTests()
    {
        _service = new CoverService(
            _mockAuditer.Object,
            _mockCoverRepository.Object,
            _mockUnitOfWork.Object,
            _mockPremiumCalculatorService.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidCover_CreatesCoverWithCalculatedPremium()
    {
        // Arrange
        var createCoverDto = new CreateCoverDto
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30),
            Type = CoverType.Yacht,
            Premium = 0 // Will be overridden by calculator
        };

        var calculatedPremium = 41250m; // Expected premium from calculator
        _mockPremiumCalculatorService.Setup(x => x.ComputePremium(
            createCoverDto.StartDate, 
            createCoverDto.EndDate, 
            createCoverDto.Type))
            .Returns(calculatedPremium);

        // Act
        var result = await _service.CreateAsync(createCoverDto);

        // Assert
        Assert.Equal(calculatedPremium, result.Premium);
        Assert.Equal(createCoverDto.Type, result.Type);
        Assert.NotNull(result.Id);
        
        _mockCoverRepository.Verify(x => x.AddItemAsync(It.IsAny<CoverEntity>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(default), Times.Once);
        _mockAuditer.Verify(x => x.AuditCover(It.IsAny<string>(), "POST"), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ExistingCover_ReturnsCoverDto()
    {
        // Arrange
        var coverId = "cover-123";
        var coverEntity = new CoverEntity
        {
            Id = coverId,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
            Type = CoverType.PassengerShip,
            Premium = 50000m
        };

        _mockCoverRepository.Setup(x => x.GetCoverOrNullAsync(coverId))
            .ReturnsAsync(coverEntity);

        // Act
        var result = await _service.GetAsync(coverId);

        // Assert
        Assert.Equal(coverEntity.Id, result.Id);
        Assert.Equal(coverEntity.Type, result.Type);
        Assert.Equal(coverEntity.Premium, result.Premium);
        Assert.Equal(coverEntity.StartDate, result.StartDate);
        Assert.Equal(coverEntity.EndDate, result.EndDate);
    }
}
