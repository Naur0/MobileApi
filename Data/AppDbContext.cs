using Microsoft.EntityFrameworkCore;
using MobileApi.Models;

namespace MobileApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<LostItemReport> LostItemReports => Set<LostItemReport>();
    }
}
