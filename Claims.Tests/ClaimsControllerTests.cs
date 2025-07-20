using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Claims.Features.Claims.Models;
using Claims.Features.Covers.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Claims.Tests;

public class ClaimsControllerTests
{
    [Fact]
    public async Task Get_Claims_EmptyDatabase_ReturnsEmptyArray()
    {
        // Arrange
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(_ => { });

        var client = application.CreateClient();

        // Act
        var response = await client.GetAsync("/Claims");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        // Verify response is valid JSON
        Assert.NotNull(responseString);

        // Verify response is an empty array when no claims exist
        var claims = JsonSerializer.Deserialize<ClaimDto[]>(responseString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });

        Assert.NotNull(claims);
        Assert.Empty(claims);

        // Verify content type is JSON
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Get_Claims_WithData_ReturnsClaimsArray()
    {
        // Arrange
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(_ => { });

        var client = application.CreateClient();

        // First create a cover (required for claim)
        var createCoverDto = new CreateCoverDto
        {
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddYears(1),
            Type = CoverType.Yacht,
            Premium = 1000.00m
        };

        var coverJson = JsonSerializer.Serialize(createCoverDto, new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        });
        var coverContent = new StringContent(coverJson, Encoding.UTF8, "application/json");

        var coverResponse = await client.PostAsync("/Covers", coverContent);
        coverResponse.EnsureSuccessStatusCode();

        var coverResponseString = await coverResponse.Content.ReadAsStringAsync();
        var cover = JsonSerializer.Deserialize<CoverDto>(coverResponseString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });

        Assert.NotNull(cover);
        Assert.NotNull(cover.Id);

        // Now create a claim
        var createClaimDto = new CreateClaimDto
        {
            CoverId = cover.Id,
            Name = "Test Collision Claim",
            Type = ClaimType.Collision,
            DamageCost = 5000.00m
        };

        var claimJson = JsonSerializer.Serialize(createClaimDto, new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        });
        var claimContent = new StringContent(claimJson, Encoding.UTF8, "application/json");

        var createClaimResponse = await client.PostAsync("/Claims", claimContent);
        createClaimResponse.EnsureSuccessStatusCode();

        // Act - Get all claims
        var response = await client.GetAsync("/Claims");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        // Verify response is valid JSON
        Assert.NotNull(responseString);

        // Verify response contains the created claim
        var claims = JsonSerializer.Deserialize<ClaimDto[]>(responseString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });

        Assert.NotNull(claims);
        Assert.Single(claims); // Should have exactly one claim

        var claim = claims[0];
        Assert.NotNull(claim.Id);
        Assert.Equal(cover.Id, claim.CoverId);
        Assert.Equal("Test Collision Claim", claim.Name);
        Assert.Equal(ClaimType.Collision, claim.Type);
        Assert.Equal(5000.00m, claim.DamageCost);

        // Verify content type is JSON
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }
}