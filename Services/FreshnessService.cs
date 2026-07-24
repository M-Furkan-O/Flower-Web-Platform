using FlowerShop.API.DTOs;
using FlowerShop.API.Models;

namespace FlowerShop.API.Services;

public class FreshnessService
{
    public FreshnessInfoDto CalculateFreshness(Product product, DateTime? deliveryDate = null)
    {
        if (product.Stock <= 0)
        {
            return new FreshnessInfoDto
            {
                FreshnessScore = 0,
                EstimatedVaseLifeDays = product.DefaultVaseLifeDays,
                RemainingVaseLifeDays = 0,
                FreshnessPercentage = 0
            };
        }

        var targetDate = deliveryDate ?? DateTime.UtcNow;
        var daysSinceDelivery = (targetDate - DateTime.UtcNow).Days;

        // Kalite bileşeni (%55): florist puanı 1–10
        var quality = (Math.Clamp(product.FreshnessScore, 1, 10) / 10.0) * 55;

        // Stok bileşeni (%25): az stok hafif düşüş
        var stock = product.Stock switch
        {
            <= 2 => 12.0,
            <= 5 => 18.0,
            <= 10 => 22.0,
            _ => 25.0
        };

        // Vazo ömrü bileşeni (%20)
        var vase = Math.Min(20.0, (product.DefaultVaseLifeDays / 10.0) * 20);

        var basePercentage = Math.Min(100.0, quality + stock + vase);

        // Teslimat tarihi geçmişse günlük %3 düşüş
        if (daysSinceDelivery > 0)
        {
            basePercentage = Math.Max(0, basePercentage - (daysSinceDelivery * 3));
        }

        var estimatedVaseLife = product.DefaultVaseLifeDays;
        var remainingVaseLife = Math.Max(0, estimatedVaseLife - daysSinceDelivery);

        return new FreshnessInfoDto
        {
            FreshnessScore = (int)Math.Round(basePercentage / 10.0),
            EstimatedVaseLifeDays = estimatedVaseLife,
            RemainingVaseLifeDays = remainingVaseLife,
            FreshnessPercentage = Math.Round(basePercentage, 1)
        };
    }

    public List<FreshnessInfoDto> CalculateFreshnessForProducts(List<Product> products, DateTime? deliveryDate = null)
    {
        return products.Select(p => CalculateFreshness(p, deliveryDate)).ToList();
    }
}
