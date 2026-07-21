namespace FlowerShop.API.Models;

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int DistrictId { get; set; }
    public District? District { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal GrandTotal { get; set; }
    public string OrderStatus { get; set; } = "Hazırlanıyor";
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public List<OrderItem> OrderItems { get; set; } = new();
}
