using Claims.Features.Claims.Models;

namespace Claims.Features.Claims.Repositories;

public interface IClaimsRepository
{
    Task<IEnumerable<ClaimEntity>> GetClaimsAsync();
    Task<ClaimEntity?> GetClaimOrNullAsync(string id);
    Task AddItemAsync(ClaimEntity item);
    Task DeleteItemAsync(string id);
}