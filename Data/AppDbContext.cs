
using Microsoft.EntityFrameworkCore;
using ModoDemoMVP.Models;

namespace ModoDemoMVP.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) { }
        public DbSet<Pago> Pagos => Set<Pago>();

    }
}
