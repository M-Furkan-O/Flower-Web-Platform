using System.Text.Json;
using FlowerShop.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.API.Data;

public static class DbSeeder
{
    public static async Task SeedCitiesAndDistrictsAsync(AppDbContext context)
    {
        // 1. Veritabanında tam 81 il varsa işlem yapma
        if (await context.Cities.CountAsync() == 81) return;

        // 2. Eğer 81 il yoksa (eski/boş veri kalmışsa) her şeyi temizle ve baştan kur
        context.Neighborhoods.RemoveRange(context.Neighborhoods);
        context.Districts.RemoveRange(context.Districts);
        context.Cities.RemoveRange(context.Cities);
        await context.SaveChangesAsync();

        // 3. JSON dosyasını oku
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "turkey_data.json");
        if (!File.Exists(jsonPath))
        {
            jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "turkey_data.json");
        }

        if (File.Exists(jsonPath))
        {
            try
            {
                var jsonString = await File.ReadAllTextAsync(jsonPath);
                var cityDtos = JsonSerializer.Deserialize<List<CityJsonDto>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (cityDtos != null && cityDtos.Any())
                {
                    int districtIdCounter = 1;
                    var citiesToAdd = new List<City>();
                    var districtsToAdd = new List<District>();

                    foreach (var cDto in cityDtos)
                    {
                        citiesToAdd.Add(new City { Id = cDto.Id, Name = cDto.Name });

                        foreach (var distName in cDto.Districts)
                        {
                            districtsToAdd.Add(new District
                            {
                                Id = districtIdCounter++,
                                CityId = cDto.Id,
                                Name = distName,
                                BaseDeliveryFee = 0
                            });
                        }
                    }

                    await context.Cities.AddRangeAsync(citiesToAdd);
                    await context.Districts.AddRangeAsync(districtsToAdd);
                    await context.SaveChangesAsync();

                    Console.WriteLine("==================================================");
                    Console.WriteLine("🚀 81 İL VE TÜM İLÇELER BAŞARIYLA YÜKLENDİ!");
                    Console.WriteLine("==================================================");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ JSON Okuma Hatası: {ex.Message}");
            }
        }

        // FALLBACK: JSON yoksa Trendyol örnek verilerini bas
        var ankara = new City { Id = 6, Name = "Ankara" };
        var istanbul = new City { Id = 34, Name = "İstanbul" };
        await context.Cities.AddRangeAsync(ankara, istanbul);
        await context.SaveChangesAsync();

        var cankaya = new District { Id = 1, CityId = 6, Name = "Çankaya", BaseDeliveryFee = 0 };
        var kadikoy = new District { Id = 2, CityId = 34, Name = "Kadıköy", BaseDeliveryFee = 20 };
        await context.Districts.AddRangeAsync(cankaya, kadikoy);
        await context.SaveChangesAsync();

        Console.WriteLine("==================================================");
        Console.WriteLine("🚀 ÖRNEK ADRES VERİLERİ VERİTABANINA YÜKLENDİ!");
        Console.WriteLine("==================================================");
    }

    private class CityJsonDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> Districts { get; set; } = new();
    }
}