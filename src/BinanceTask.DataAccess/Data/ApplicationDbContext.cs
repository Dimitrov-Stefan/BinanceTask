using Microsoft.EntityFrameworkCore;

namespace BinanceTask.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        DbSet<PriceData> PriceData { get; set; }
    }
}