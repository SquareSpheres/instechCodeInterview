using Claims.Core.Contexts;
using Claims.Features.Claims.Models;
using Microsoft.EntityFrameworkCore;

namespace Claims.Features.Claims.Repositories;

public class ClaimsRepository(ClaimsContext claimsContext, ILogger<ClaimsRepository> logger) : IClaimsRepository
{
    public async Task<IEnumerable<ClaimEntity>> GetClaimsAsync()
    {
        return await claimsContext.Claims.ToListAsync();
    }

    public async Task<ClaimEntity?> GetClaimOrNullAsync(string id)
    {
        return await claimsContext.Claims
            .Where(claim => claim.Id == id)
            .SingleOrDefaultAsync();
    }

    public async Task AddItemAsync(ClaimEntity item)
    {
        await claimsContext.Claims.AddAsync(item);
    }

    public async Task DeleteItemAsync(string id)
    {
        var claim = await GetClaimOrNullAsync(id);
        if (claim is not null)
            claimsContext.Claims.Remove(claim);
        else
            logger.LogWarning("Attempted to delete non-existent claim: {ClaimId}", id);
    }
}