namespace Claims.Features.Covers.Models;

public class CoverDto
{
    public required string Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public CoverType Type { get; set; }
    public decimal Premium { get; set; }
}