using Claims.Features.Covers.Models;

namespace Claims.Features.Covers.Services;

public interface IPremiumTier
{
    decimal CalculatePremium(int numberOfDays, decimal basePremiumPerDay, CoverType coverType);
}