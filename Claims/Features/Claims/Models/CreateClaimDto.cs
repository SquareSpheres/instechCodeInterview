namespace Claims.Features.Claims.Models;

public class CreateClaimDto
{
    public required string CoverId { get; set; }
    public required string Name { get; set; }
    public ClaimType Type { get; set; }
    public decimal DamageCost { get; set; }
}