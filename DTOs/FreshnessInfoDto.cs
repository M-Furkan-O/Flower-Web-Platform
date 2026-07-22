namespace FlowerShop.API.DTOs;

public class FreshnessInfoDto
{
    public int FreshnessScore { get; set; } // 1-10 scale
    public int EstimatedVaseLifeDays { get; set; }
    public int RemainingVaseLifeDays { get; set; }
    public double FreshnessPercentage { get; set; } // For progress bar (0-100)
}
