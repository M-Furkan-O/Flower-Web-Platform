namespace FlowerShop.API.Models;

public class StoreContact
{
    public int Id { get; set; }
    public string Phone { get; set; } = string.Empty;       // Örn: "905XXXXXXXXX"
    public string WhatsAppPhone { get; set; } = string.Empty; // WhatsApp Hattı
    public string? Email { get; set; }                       // Mağaza E-postası
    public string? Address { get; set; }                     // Mağaza Adresi
}