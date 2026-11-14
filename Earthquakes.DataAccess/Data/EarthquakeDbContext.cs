using Earthquakes.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Earthquakes.DataAccess.Data;

public class EarthquakeDbContext : DbContext
{
    public EarthquakeDbContext(DbContextOptions<EarthquakeDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Earthquake> Earthquakes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Earthquake>(entity =>
        {
            entity.ToTable("Earthquakes");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.UsgsId).HasMaxLength(50);
            entity.Property(e => e.Magnitude).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.Place).HasMaxLength(255);
            entity.Property(e => e.Time).HasColumnType("datetime");
            entity.Property(e => e.Url).HasMaxLength(500);
        });
    }
}
