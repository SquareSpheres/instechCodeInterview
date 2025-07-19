namespace Claims.Features.Covers.Services.PremiumTiers;

public class PremiumTierFactory : IPremiumTierFactory
{
    public IEnumerable<IPremiumTier> CreatePremiumTiers()
    {
        return new List<IPremiumTier>
        {
            new FirstTierPremiumCalculator(),
            new SecondTierPremiumCalculator(),
            new ThirdTierPremiumCalculator()
        };
    }
}