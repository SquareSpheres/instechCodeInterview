using Claims.Features.Covers.Models;

namespace Claims.Features.Covers.Services;

public class ThirdTierPremiumCalculator : IPremiumTier
{
    private const int TierStartDay = 181;
    private const decimal YachtSecondTierDiscount = 0.05m;
    private const decimal OtherTypesSecondTierDiscount = 0.02m;
    private const decimal YachtAdditionalDiscount = 0.03m;
    private const decimal OtherTypesAdditionalDiscount = 0.01m;

    public decimal CalculatePremium(int numberOfDays, decimal basePremiumPerDay, CoverType coverType)
    {
        if (numberOfDays <= TierStartDay - 1)
            return 0m;
            
        var daysInTier = numberOfDays - (TierStartDay - 1);
        
        var secondTierDiscount = coverType == CoverType.Yacht ? YachtSecondTierDiscount : OtherTypesSecondTierDiscount;
        var additionalDiscount = coverType == CoverType.Yacht ? YachtAdditionalDiscount : OtherTypesAdditionalDiscount;
        var totalDiscount = secondTierDiscount + additionalDiscount;
        var discountedRate = basePremiumPerDay * (1 - totalDiscount);
        
        return daysInTier * discountedRate;
    }
}
