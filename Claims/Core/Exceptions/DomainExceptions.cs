namespace Claims.Core.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class ClaimNotFoundException : DomainException
{
    public string ClaimId { get; }
    
    public ClaimNotFoundException(string claimId) 
        : base($"Claim with ID '{claimId}' was not found")
    {
        ClaimId = claimId;
    }
}

public class CoverNotFoundException : DomainException
{
    public string CoverId { get; }
    
    public CoverNotFoundException(string coverId) 
        : base($"Cover with ID '{coverId}' was not found")
    {
        CoverId = coverId;
    }
}

public class CoverageValidationException : DomainException
{
    public string CoverId { get; }
    public DateOnly CurrentDate { get; }
    public DateOnly StartDate { get; }
    public DateOnly EndDate { get; }
    
    public CoverageValidationException(string coverId, DateOnly currentDate, DateOnly startDate, DateOnly endDate) 
        : base($"Claims can only be made during the coverage period ({startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}). Current date: {currentDate:yyyy-MM-dd}")
    {
        CoverId = coverId;
        CurrentDate = currentDate;
        StartDate = startDate;
        EndDate = endDate;
    }
}

public class InvalidDateRangeException : DomainException
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    
    public InvalidDateRangeException(DateTime startDate, DateTime endDate) 
        : base($"Invalid date range: start date ({startDate:yyyy-MM-dd}) must be before end date ({endDate:yyyy-MM-dd})")
    {
        StartDate = startDate;
        EndDate = endDate;
    }
}
