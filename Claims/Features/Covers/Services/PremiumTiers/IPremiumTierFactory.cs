namespace Claims.Features.Covers.Services.PremiumTiers;

public interface IPremiumTierFactory
{
    IEnumerable<IPremiumTier> CreatePremiumTiers();
}