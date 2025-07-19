using Claims.Features.Claims.Models;

namespace Claims.Features.Claims.Services;

public interface IClaimsService
{
    Task<IEnumerable<ClaimDto>> GetAllAsync();
    Task<ClaimDto> CreateAsync(CreateClaimDto dto);
    Task DeleteAsync(string id);
    Task<ClaimDto> GetAsync(string id);
}