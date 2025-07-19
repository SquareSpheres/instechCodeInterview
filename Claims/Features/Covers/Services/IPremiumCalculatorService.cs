using Claims.Features.Covers.Models;

namespace Claims.Features.Covers.Services;

public interface IPremiumCalculatorService
{
    decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
}