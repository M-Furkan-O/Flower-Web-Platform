using FlowerShop.API.Data;
using FlowerShop.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly AppDbContext _context;

    public ContactController(AppDbContext context)
    {
        _context = context;
    }

    // 🟢 GET /api/Contact/whatsapp (Müşterinin frontend'den çağıracağı endpoint)
    [HttpGet("whatsapp")]
    public async Task<ActionResult<object>> GetWhatsAppPhone()
    {
        var contact = await _context.StoreContacts.FirstOrDefaultAsync();
        
        // Veritabanında henüz veri yoksa fallback olarak varsayılan döner
        var phone = contact?.WhatsAppPhone ?? "905551112233";

        return Ok(new { whatsAppPhone = phone });
    }

    // 🟡 PUT /api/Contact/whatsapp (Admin'in numarayı değiştireceği endpoint)
    [HttpPut("whatsapp")]
    public async Task<IActionResult> UpdateWhatsAppPhone([FromBody] string newPhone)
    {
        var contact = await _context.StoreContacts.FirstOrDefaultAsync();

        if (contact == null)
        {
            contact = new StoreContact { WhatsAppPhone = newPhone };
            _context.StoreContacts.Add(contact);
        }
        else
        {
            contact.WhatsAppPhone = newPhone;
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "WhatsApp numarası güncellendi!", phone = newPhone });
    }
}