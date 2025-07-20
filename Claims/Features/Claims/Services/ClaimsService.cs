using Claims.Auditing;
using Claims.Core.Exceptions;
using Claims.Core.Infrastructure;
using Claims.Features.Claims.Mappers;
using Claims.Features.Claims.Models;
using Claims.Features.Claims.Repositories;
using Claims.Features.Covers.Repositories;

namespace Claims.Features.Claims.Services;

public class ClaimsService(
    IAuditer auditer,
    IClaimsRepository claimsRepository,
    ICoverRepository coverRepository,
    IUnitOfWork unitOfWork,
    ILogger<ClaimsService> logger)
    : IClaimsService
{
    // TODO paging?
    public async Task<IEnumerable<ClaimDto>> GetAllAsync()
    {
        var claimEntity = await claimsRepository.GetClaimsAsync();
        return claimEntity.Select(c => c.ToDto()).ToList();
    }

    public async Task<ClaimDto> CreateAsync(CreateClaimDto dto)
    {
        await ValidateCoveragePeriodAsync(dto);

        var claimEntity = dto.ToEntity();
        claimEntity.Id = Guid.NewGuid().ToString();
        await claimsRepository.AddItemAsync(claimEntity);
        await unitOfWork.SaveChangesAsync();
        auditer.AuditClaim(claimEntity.Id, "POST");

        logger.LogInformation("Created claim {ClaimId} for cover {CoverId} with damage cost {DamageCost}",
            claimEntity.Id, dto.CoverId, dto.DamageCost);

        return claimEntity.ToDto();
    }

    public async Task DeleteAsync(string id)
    {
        auditer.AuditClaim(id, "DELETE");
        await claimsRepository.DeleteItemAsync(id);
        await unitOfWork.SaveChangesAsync();
    }


    public async Task<ClaimDto> GetAsync(string id)
    {
        var claim = await claimsRepository.GetClaimOrNullAsync(id);
        if (claim == null) throw new ClaimNotFoundException(id);

        return claim.ToDto();
    }

    private async Task ValidateCoveragePeriodAsync(CreateClaimDto dto)
    {
        var cover = await coverRepository.GetCoverOrNullAsync(dto.CoverId);

        if (cover == null) throw new CoverNotFoundException(dto.CoverId);

        var today = DateOnly.FromDateTime(DateTime.Now.Date);
        if (today < cover.StartDate || today > cover.EndDate)
            throw new CoverageValidationException(dto.CoverId, today, cover.StartDate, cover.EndDate);
    }
}