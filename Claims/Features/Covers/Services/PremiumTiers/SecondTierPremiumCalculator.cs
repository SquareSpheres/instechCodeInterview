using Claims.Features.Covers.Models;

namespace Claims.Features.Covers.Services;

public class SecondTierPremiumCalculator : IPremiumTier
{
    private const int TierStartDay = 31;
    private const int TierEndDay = 180;
    private const decimal YachtDiscount = 0.05m;
    private const decimal OtherTypesDiscount = 0.02m;

    public decimal CalculatePremium(int numberOfDays, decimal basePremiumPerDay, CoverType coverType)
    {
        // Following 150 days (days 31-180) with discount
        if (numberOfDays <= TierStartDay - 1)
            return 0m;
            
        var daysInTier = Math.Min(numberOfDays, TierEndDay) - (TierStartDay - 1);
        if (daysInTier <= 0)
            return 0m;
            
        var discount = coverType == CoverType.Yacht ? YachtDiscount : OtherTypesDiscount;
        var discountedRate = basePremiumPerDay * (1 - discount);
        
        return daysInTier * discountedRate;
    }
}
