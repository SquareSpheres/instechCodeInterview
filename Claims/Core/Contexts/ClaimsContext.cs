using Claims.Features.Claims.Models;
using Claims.Features.Covers.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Core.Contexts;

public class ClaimsContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<ClaimEntity> Claims { get; init; }
    public DbSet<CoverEntity> Covers { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ClaimEntity>().ToCollection("claims");
        modelBuilder.Entity<CoverEntity>().ToCollection("covers");
    }
}