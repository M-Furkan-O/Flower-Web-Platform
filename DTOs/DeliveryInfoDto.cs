namespace FlowerShop.API.DTOs;

public class DeliveryInfoDto
{
    public decimal BaseFee { get; set; }
    public double DistanceKm { get; set; }
    public decimal PerKmRate { get; set; }
    public decimal TotalDeliveryFee { get; set; }
}
