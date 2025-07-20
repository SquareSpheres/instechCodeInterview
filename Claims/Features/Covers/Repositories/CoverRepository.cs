using Claims.Core.Contexts;
using Claims.Features.Covers.Models;
using Microsoft.EntityFrameworkCore;

namespace Claims.Features.Covers.Repositories;

public class CoverRepository(ClaimsContext claimsContext, ILogger<CoverRepository> logger) : ICoverRepository
{
    public async Task<IEnumerable<CoverEntity>> GetCoversAsync()
    {
        return await claimsContext.Covers.ToListAsync();
    }

    public async Task<CoverEntity?> GetCoverOrNullAsync(string id)
    {
        return await claimsContext.Covers
            .Where(cover => cover.Id == id)
            .SingleOrDefaultAsync();
    }

    public async Task AddItemAsync(CoverEntity item)
    {
        await claimsContext.Covers.AddAsync(item);
    }

    public async Task DeleteItemAsync(string id)
    {
        var cover = await GetCoverOrNullAsync(id);
        if (cover is not null) 
        {
            claimsContext.Covers.Remove(cover);
        }
        else
        {
            logger.LogWarning("Attempted to delete non-existent cover: {CoverId}", id);
        }
    }
}