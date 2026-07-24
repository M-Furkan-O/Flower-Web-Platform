namespace FlowerShop.API.Models;

public class Neighborhood
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Pk { get; set; } = string.Empty; // Posta Kodu

    // İlçe İlişkisi
    public int DistrictId { get; set; }
    public District District { get; set; } = null!;
}