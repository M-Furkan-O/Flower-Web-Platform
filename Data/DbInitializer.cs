using FlowerShop.API.Models;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace FlowerShop.API.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Category
                {
                    Name = "Güller",
                    Description = "Taze ve canlı gül aranjmanları"
                },
                new Category
                {
                    Name = "Orkideler",
                    Description = "Saksıda uzun ömürlü zarif orkideler"
                },
                new Category
                {
                    Name = "Saksı Çiçekleri",
                    Description = "Ev ve ofis için dekoratif iç mekan bitkileri"
                });

            context.SaveChanges();
        }

        if (!context.Products.Any())
        {
            var categories = context.Categories.ToDictionary(c => c.Name);

            context.Products.AddRange(
                new Product
                {
                    Name = "Kırmızı Gül Buketi",
                    Price = 450,
                    Stock = 25,
                    FreshnessScore = 9,
                    ImageUrl = "https://images.unsplash.com/photo-1518895949257-7621c3c786d8?w=400",
                    CategoryId = categories["Güller"].Id
                },
                new Product
                {
                    Name = "Premium Beyaz Gül Aranjmanı",
                    Price = 520,
                    Stock = 18,
                    FreshnessScore = 10,
                    ImageUrl = "https://images.unsplash.com/photo-1490750967868-88aa4486c946?w=400",
                    CategoryId = categories["Güller"].Id
                },
                new Product
                {
                    Name = "Phalaenopsis Beyaz Orkide",
                    Price = 380,
                    Stock = 12,
                    FreshnessScore = 9,
                    ImageUrl = "https://images.unsplash.com/photo-1518709268805-4e9042af2179?w=400",
                    CategoryId = categories["Orkideler"].Id
                },
                new Product
                {
                    Name = "Monstera Deliciosa",
                    Price = 290,
                    Stock = 15,
                    FreshnessScore = 8,
                    ImageUrl = "https://images.unsplash.com/photo-1614594975525-e45190c55d0a?w=400",
                    CategoryId = categories["Saksı Çiçekleri"].Id
                });

            context.SaveChanges();
        }

        if (!context.Districts.Any())
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            context.Districts.AddRange(
                new District
                {
                    Name = "Çankaya",
                    BaseDeliveryFee = 50,
                    Location = geometryFactory.CreatePoint(new Coordinate(32.8597, 39.9208))
                },
                new District
                {
                    Name = "Yenimahalle",
                    BaseDeliveryFee = 60,
                    Location = geometryFactory.CreatePoint(new Coordinate(32.8123, 39.9678))
                },
                new District
                {
                    Name = "Keçiören",
                    BaseDeliveryFee = 65,
                    Location = geometryFactory.CreatePoint(new Coordinate(32.8644, 39.9782))
                },
                new District
                {
                    Name = "Polatlı",
                    BaseDeliveryFee = 120,
                    Location = geometryFactory.CreatePoint(new Coordinate(32.1481, 39.5833))
                });

            context.SaveChanges();
        }
    }
}
