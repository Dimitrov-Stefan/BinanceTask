using BinanceTask.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BinanceTask.Infrastructure.Data;

/// <summary>
/// The application database context.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PriceData>()
            .Property(pd => pd.Price).HasPrecision(18, 8);

        base.OnModelCreating(modelBuilder);
    }

    DbSet<PriceData> PriceData { get; set; }
}