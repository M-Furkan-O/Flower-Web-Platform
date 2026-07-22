namespace FlowerShop.API.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int FreshnessScore { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public int DefaultVaseLifeDays { get; set; } = 7;
    public List<WikiNote> WikiNotes { get; set; } = new();
}
