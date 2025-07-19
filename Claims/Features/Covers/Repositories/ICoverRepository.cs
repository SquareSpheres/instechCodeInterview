using Claims.Features.Covers.Models;

namespace Claims.Features.Covers.Repositories;

public interface ICoverRepository
{
    Task<IEnumerable<CoverEntity>> GetCoversAsync();
    Task<CoverEntity?> GetCoverOrNullAsync(string id);
    Task AddItemAsync(CoverEntity item);
    Task DeleteItemAsync(string id);
}