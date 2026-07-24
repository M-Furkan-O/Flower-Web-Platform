namespace FlowerShop.API.DTOs;

public class CreateDistrictDto
{
    public string Name { get; set; } = string.Empty;
    public decimal BaseDeliveryFee { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
