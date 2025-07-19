using Claims.Auditing;
using Claims.Core.Infrastructure;
using Claims.Features.Covers.Mappers;
using Claims.Features.Covers.Models;
using Claims.Features.Covers.Repositories;

namespace Claims.Features.Covers.Services;

public class CoverService(
    IAuditer auditer,
    ICoverRepository coverRepository,
    IUnitOfWork unitOfWork,
    IPremiumCalculatorService premiumCalculatorService) : ICoverService
{
    public async Task<IEnumerable<CoverDto>> GetAllAsync()
    {
        var coverEntities = await coverRepository.GetCoversAsync();
        return coverEntities.Select(c => c.ToDto()).ToList();
    }

    public async Task<CoverDto> GetAsync(string id)
    {
        var cover = await coverRepository.GetCoverOrNullAsync(id);
        if (cover == null) throw new KeyNotFoundException($"Cover with id {id} not found");

        return cover.ToDto();
    }

    public async Task<CoverDto> CreateAsync(CreateCoverDto dto)
    {
        var coverEntity = dto.ToEntity();
        coverEntity.Id = Guid.NewGuid().ToString();
        coverEntity.Premium = premiumCalculatorService.ComputePremium(dto.StartDate, dto.EndDate, dto.Type);

        await coverRepository.AddItemAsync(coverEntity);
        await unitOfWork.SaveChangesAsync();
        auditer.AuditCover(coverEntity.Id, "POST");

        return coverEntity.ToDto();
    }

    public async Task DeleteAsync(string id)
    {
        auditer.AuditCover(id, "DELETE");
        await coverRepository.DeleteItemAsync(id);
        await unitOfWork.SaveChangesAsync();
    }

    public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        return premiumCalculatorService.ComputePremium(startDate, endDate, coverType);
    }
}