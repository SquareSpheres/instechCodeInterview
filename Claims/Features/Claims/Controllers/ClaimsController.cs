using Claims.Features.Claims.Models;
using Claims.Features.Claims.Services;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Features.Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController(
    IClaimsService claimsService
)
    : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<ClaimDto>> GetAsync()
    {
        return await claimsService.GetAllAsync();
    }

    [HttpPost]
    public async Task<ActionResult<ClaimDto>> CreateAsync(CreateClaimDto createClaimDto)
    {
        var claim = await claimsService.CreateAsync(createClaimDto);
        return Ok(claim);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
    {
        await claimsService.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public async Task<ClaimDto> GetAsync(string id)
    {
        return await claimsService.GetAsync(id);
    }
}