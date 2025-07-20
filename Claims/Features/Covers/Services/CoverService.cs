using Claims.Auditing;
using Claims.Core.Infrastructure;
using Claims.Core.Exceptions;
using Claims.Features.Covers.Mappers;
using Claims.Features.Covers.Models;
using Claims.Features.Covers.Repositories;

namespace Claims.Features.Covers.Services;

public class CoverService(
    IAuditer auditer,
    ICoverRepository coverRepository,
    IUnitOfWork unitOfWork,
    IPremiumCalculatorService premiumCalculatorService,
    ILogger<CoverService> logger) : ICoverService
{
    public async Task<IEnumerable<CoverDto>> GetAllAsync()
    {
        var coverEntities = await coverRepository.GetCoversAsync();
        return coverEntities.Select(c => c.ToDto()).ToList();
    }

    public async Task<CoverDto> GetAsync(string id)
    {
        var cover = await coverRepository.GetCoverOrNullAsync(id);
        if (cover == null)
        {
            throw new CoverNotFoundException(id);
        }

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

        logger.LogInformation("Created cover {CoverId} of type {CoverType} with premium {Premium} for period {StartDate} to {EndDate}", 
            coverEntity.Id, dto.Type, coverEntity.Premium, dto.StartDate, dto.EndDate);

        return coverEntity.ToDto();
    }

    public async Task DeleteAsync(string id)
    {
        auditer.AuditCover(id, "DELETE");
        await coverRepository.DeleteItemAsync(id);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Deleted cover {CoverId}", id);
    }

    public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        return premiumCalculatorService.ComputePremium(startDate, endDate, coverType);
    }
}