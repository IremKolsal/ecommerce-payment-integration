using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Order>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).HasMaxLength(32).IsRequired();
            e.Property(x => x.TotalAmount).HasColumnType("numeric(18,2)");
            e.Property(x => x.CreatedAt).IsRequired();

            e.HasMany(x => x.Items)
             .WithOne()
             .HasForeignKey(x => x.OrderId);
        });

        b.Entity<OrderItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ProductId).HasMaxLength(64).IsRequired();
            e.Property(x => x.Quantity).IsRequired();
            e.Property(x => x.UnitPrice).HasColumnType("numeric(18,2)");
        });
    }
}
