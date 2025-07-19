using Microsoft.EntityFrameworkCore;

namespace Claims.Auditing;

public class AuditContext(DbContextOptions<AuditContext> options) : DbContext(options)
{
    public DbSet<ClaimAuditEntity> ClaimAudits { get; set; }
    public DbSet<CoverAuditEntity> CoverAudits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClaimAuditEntity>().ToTable("ClaimAudits");
        modelBuilder.Entity<CoverAuditEntity>().ToTable("CoverAudits");
    }
}