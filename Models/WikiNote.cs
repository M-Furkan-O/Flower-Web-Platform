namespace FlowerShop.API.Models;

public class WikiNote
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // e.g., "Watering", "Climate", "Light"
    public List<Product> Products { get; set; } = new();
}
