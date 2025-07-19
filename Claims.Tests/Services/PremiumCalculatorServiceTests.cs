using Claims.Features.Covers.Models;
using Claims.Features.Covers.Services;
using Claims.Features.Covers.Services.PremiumTiers;
using Xunit;

namespace Claims.Tests.Services;

public class PremiumCalculatorServiceTests
{
    private readonly PremiumCalculatorService _service = new(new PremiumTierFactory());

    [Fact]
    public void ComputePremium_YachtFor30Days_ReturnsCorrectBasePremium()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 31); // 30 days
        var coverType = CoverType.Yacht;

        // Act
        var premium = _service.ComputePremium(startDate, endDate, coverType);

        // Assert
        var expected = 30 * 1250 * 1.1m; // 30 days * base rate * yacht multiplier
        Assert.Equal(expected, premium);
    }

    [Fact]
    public void ComputePremium_TankerFor200Days_AppliesDiscountsCorrectly()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 7, 19); // ~200 days
        var coverType = CoverType.Tanker;
        var basePremiumPerDay = 1250 * 1.5m; // Tanker multiplier

        // Act
        var premium = _service.ComputePremium(startDate, endDate, coverType);

        // Assert
        var insuranceDays = (endDate - startDate).TotalDays;
        var first30Days = 30 * basePremiumPerDay;
        var next150Days = 150 * basePremiumPerDay * 0.98m; // 2% discount for non-yacht
        var remainingDays = ((int)insuranceDays - 180) * basePremiumPerDay * 0.97m; // Additional 1% discount for non-yacht
        var expected = first30Days + next150Days + remainingDays;
        
        Assert.Equal(expected, premium);
    }
    
    [Theory]
    [InlineData(CoverType.Yacht, 1.1)]
    [InlineData(CoverType.PassengerShip, 1.2)]
    [InlineData(CoverType.Tanker, 1.5)]
    [InlineData(CoverType.ContainerShip, 1.3)]
    public void ComputePremium_DifferentCoverTypes_UsesCorrectMultiplier(CoverType coverType, decimal expectedMultiplier)
    {
        // Test 1-day premium to isolate the multiplier
        var premium = _service.ComputePremium(DateTime.Today, DateTime.Today.AddDays(1), coverType);
        var expected = 1250m * expectedMultiplier;
        Assert.Equal(expected, premium);
    }
}
