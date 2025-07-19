using Claims.Features.Covers.Models;

namespace Claims.Features.Covers.Services;

public interface ICoverService
{
    Task<IEnumerable<CoverDto>> GetAllAsync();
    Task<CoverDto> GetAsync(string id);
    Task<CoverDto> CreateAsync(CreateCoverDto dto);
    Task DeleteAsync(string id);
    decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
}