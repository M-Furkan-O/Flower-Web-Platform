namespace FlowerShop.API.DTOs;

public class OrderItemResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public OrderProductDto? Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
