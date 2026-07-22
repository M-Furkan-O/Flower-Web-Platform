namespace FlowerShop.API.DTOs;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int FreshnessScore { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int DefaultVaseLifeDays { get; set; } = 7;
}
