using Claims.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Claims.Core.Handlers;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            ClaimNotFoundException ex => HandleClaimNotFound(ex),
            CoverNotFoundException ex => HandleCoverNotFound(ex),
            CoverageValidationException ex => HandleCoverageValidation(ex),
            InvalidDateRangeException ex => HandleInvalidDateRange(ex),
            ArgumentException ex => HandleArgumentException(ex),
            _ => HandleGenericException(exception)
        };

        httpContext.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }

    private ProblemDetails HandleClaimNotFound(ClaimNotFoundException ex)
    {
        logger.LogWarning("Claim not found: {ClaimId}", ex.ClaimId);
        return new ProblemDetails
        {
            Status = (int)HttpStatusCode.NotFound,
            Title = "Claim Not Found",
            Detail = ex.Message,
            Extensions = { ["claimId"] = ex.ClaimId }
        };
    }

    private ProblemDetails HandleCoverNotFound(CoverNotFoundException ex)
    {
        logger.LogWarning("Cover not found: {CoverId}", ex.CoverId);
        return new ProblemDetails
        {
            Status = (int)HttpStatusCode.NotFound,
            Title = "Cover Not Found",
            Detail = ex.Message,
            Extensions = { ["coverId"] = ex.CoverId }
        };
    }

    private ProblemDetails HandleCoverageValidation(CoverageValidationException ex)
    {
        logger.LogWarning("Coverage validation failed for cover {CoverId}: Current date {CurrentDate}, Coverage: {StartDate} to {EndDate}", 
            ex.CoverId, ex.CurrentDate, ex.StartDate, ex.EndDate);
        return new ProblemDetails
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = "Coverage Period Validation Failed",
            Detail = ex.Message,
            Extensions = 
            {
                ["coverId"] = ex.CoverId,
                ["currentDate"] = ex.CurrentDate,
                ["coverageStartDate"] = ex.StartDate,
                ["coverageEndDate"] = ex.EndDate
            }
        };
    }

    private ProblemDetails HandleInvalidDateRange(InvalidDateRangeException ex)
    {
        logger.LogWarning("Invalid date range: StartDate {StartDate}, EndDate {EndDate}", 
            ex.StartDate, ex.EndDate);
        return new ProblemDetails
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = "Invalid Date Range",
            Detail = ex.Message,
            Extensions = 
            {
                ["startDate"] = ex.StartDate,
                ["endDate"] = ex.EndDate
            }
        };
    }

    private ProblemDetails HandleArgumentException(ArgumentException ex)
    {
        logger.LogWarning("Invalid argument: {Message}", ex.Message);
        return new ProblemDetails
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = "Invalid Argument",
            Detail = ex.Message
        };
    }

    private ProblemDetails HandleGenericException(Exception ex)
    {
        logger.LogError(ex, "An unexpected error occurred");
        return new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "Internal Server Error",
            Detail = "An internal server error occurred"
        };
    }
}
