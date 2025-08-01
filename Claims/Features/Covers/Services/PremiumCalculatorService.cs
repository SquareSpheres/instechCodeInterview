using Claims.Core.Exceptions;
using Claims.Features.Covers.Models;
using Claims.Features.Covers.Services.PremiumTiers;

namespace Claims.Features.Covers.Services;

public class PremiumCalculatorService(IPremiumTierFactory tierFactory, ILogger<PremiumCalculatorService> logger)
    : IPremiumCalculatorService
{
    private const decimal BaseDayRate = 1250m;
    private readonly List<IPremiumTier> _premiumTiers = tierFactory.CreatePremiumTiers().ToList();

    public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        var insuranceLengthDays = (int)Math.Ceiling((endDate - startDate).TotalDays);
        if (insuranceLengthDays <= 0) throw new InvalidDateRangeException(startDate, endDate);

        var basePremiumPerDay = GetBasePremiumPerDay(coverType);
        var totalPremium = _premiumTiers.Sum(tier =>
            tier.CalculatePremium(insuranceLengthDays, basePremiumPerDay, coverType));

        logger.LogInformation("Calculated premium {Premium} for {CoverType} over {Days} days",
            totalPremium, coverType, insuranceLengthDays);

        return totalPremium;
    }

    private static decimal GetBasePremiumPerDay(CoverType coverType)
    {
        var multiplier = GetCoverTypeMultiplier(coverType);
        return BaseDayRate * multiplier;
    }

    private static decimal GetCoverTypeMultiplier(CoverType coverType)
    {
        return coverType switch
        {
            CoverType.Yacht => 1.1m,
            CoverType.PassengerShip => 1.2m,
            CoverType.Tanker => 1.5m,
            _ => 1.3m
        };
    }
}