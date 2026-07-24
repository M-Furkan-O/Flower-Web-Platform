using NetTopologySuite.Geometries;

namespace FlowerShop.API.Models;

public class District
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal BaseDeliveryFee { get; set; }
    public Point? Location { get; set; }

    // Şehir İlişkisi
    public int CityId { get; set; }
    public City City { get; set; } = null!;
}