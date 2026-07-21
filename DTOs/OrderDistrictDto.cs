namespace FlowerShop.API.DTOs;

public class OrderDistrictDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal BaseDeliveryFee { get; set; }
}
