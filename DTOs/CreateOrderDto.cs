namespace FlowerShop.API.DTOs;

public class CreateOrderDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int DistrictId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}
