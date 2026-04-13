
using Microsoft.EntityFrameworkCore;
using ModoDemoMVP.Models;

namespace ModoDemoMVP.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) { }
        public DbSet<Pago> Pagos => Set<Pago>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pago>()
                .Property(p => p.Monto)
                .HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}
