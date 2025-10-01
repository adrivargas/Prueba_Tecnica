using Microsoft.EntityFrameworkCore;
using ProductService.Api.Entities;


namespace ProductService.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();


        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Product>(e =>
            {
                e.Property(p => p.Price).HasColumnType("decimal(18,2)");
                e.Property(p => p.RowVersion).IsRowVersion();
                e.Property(p => p.Name).HasMaxLength(120).IsRequired();
                e.Property(p => p.Category).HasMaxLength(80);
                e.Property(p => p.ImageUrl).HasMaxLength(300);
            });
        }
    }
}