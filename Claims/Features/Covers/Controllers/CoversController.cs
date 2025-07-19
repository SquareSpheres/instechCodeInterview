using Claims.Features.Covers.Models;
using Claims.Features.Covers.Services;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Features.Covers.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController(ICoverService coverService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoverDto>>> GetAsync()
    {
        var results = await coverService.GetAllAsync();
        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoverDto>> GetAsync(string id)
    {
        var result = await coverService.GetAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CoverDto>> CreateAsync(CreateCoverDto createCoverDto)
    {
        var cover = await coverService.CreateAsync(createCoverDto);
        return Ok(cover);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
    {
        await coverService.DeleteAsync(id);
    }

    [HttpPost("compute")]
    public ActionResult<decimal> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        var premium = coverService.ComputePremium(startDate, endDate, coverType);
        return Ok(premium);
    }
}