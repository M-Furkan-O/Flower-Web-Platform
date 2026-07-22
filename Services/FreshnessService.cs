using FlowerShop.API.DTOs;
using FlowerShop.API.Models;

namespace FlowerShop.API.Services;

public class FreshnessService
{
    public FreshnessInfoDto CalculateFreshness(Product product, DateTime? deliveryDate = null)
    {
        var targetDate = deliveryDate ?? DateTime.UtcNow;
        var daysSinceOrder = (targetDate - DateTime.UtcNow).Days;
        
        // Calculate freshness score based on stock and default vase life
        // Higher stock = potentially fresher (assuming FIFO inventory)
        var baseScore = Math.Min(10, Math.Max(1, product.FreshnessScore));
        
        // Adjust score based on delivery date
        // If delivery is in the future, score remains high
        // If delivery was in the past, score decreases
        var adjustedScore = (double)baseScore;
        if (daysSinceOrder > 0)
        {
            // Reduce score by 0.5 per day after delivery
            adjustedScore = Math.Max(1.0, adjustedScore - (daysSinceOrder * 0.5));
        }
        
        // Calculate estimated vase life
        var estimatedVaseLife = product.DefaultVaseLifeDays;
        var remainingVaseLife = Math.Max(0, estimatedVaseLife - daysSinceOrder);
        
        // Calculate freshness percentage for progress bar
        var freshnessPercentage = (remainingVaseLife / (double)estimatedVaseLife) * 100;
        
        return new FreshnessInfoDto
        {
            FreshnessScore = (int)Math.Round(adjustedScore),
            EstimatedVaseLifeDays = estimatedVaseLife,
            RemainingVaseLifeDays = remainingVaseLife,
            FreshnessPercentage = Math.Round((double)freshnessPercentage, 1)
        };
    }
    
    public List<FreshnessInfoDto> CalculateFreshnessForProducts(List<Product> products, DateTime? deliveryDate = null)
    {
        return products.Select(p => CalculateFreshness(p, deliveryDate)).ToList();
    }
}
