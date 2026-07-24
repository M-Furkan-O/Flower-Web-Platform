using FlowerShop.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<District> Districts => Set<District>();
    public DbSet<Neighborhood> Neighborhoods => Set<Neighborhood>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<WikiNote> WikiNotes => Set<WikiNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PostgreSQL PostGIS eklentisi
        modelBuilder.HasPostgresExtension("postgis");

        // City - District İlişkisi (1 - N)
        modelBuilder.Entity<District>()
            .HasOne(d => d.City)
            .WithMany(c => c.Districts)
            .HasForeignKey(d => d.CityId);

        // District - Neighborhood İlişkisi (1 - N)
        modelBuilder.Entity<Neighborhood>()
            .HasOne(n => n.District)
            .WithMany()
            .HasForeignKey(n => n.DistrictId);

        // Precision Ayarları
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<District>()
            .Property(d => d.BaseDeliveryFee)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(o => o.SubTotal).HasPrecision(18, 2);
            entity.Property(o => o.DeliveryFee).HasPrecision(18, 2);
            entity.Property(o => o.GrandTotal).HasPrecision(18, 2);
        });

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2);

        // Product - WikiNote (Many-to-Many) İlişkisi
        modelBuilder.Entity<Product>()
            .HasMany(p => p.WikiNotes)
            .WithMany(wn => wn.Products);
    }
}