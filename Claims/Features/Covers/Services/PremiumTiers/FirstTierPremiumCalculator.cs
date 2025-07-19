using Claims.Features.Covers.Models;

namespace Claims.Features.Covers.Services;

public class FirstTierPremiumCalculator : IPremiumTier
{
    private const int MaxDaysInTier = 30;

    public decimal CalculatePremium(int numberOfDays, decimal basePremiumPerDay, CoverType coverType)
    {
        // First 30 days at full rate
        var daysInTier = Math.Min(numberOfDays, MaxDaysInTier);
        return daysInTier * basePremiumPerDay;
    }
}
