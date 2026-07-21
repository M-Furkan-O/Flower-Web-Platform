using FlowerShop.API.Data;
using FlowerShop.API.DTOs;
using FlowerShop.API.Models;
using FlowerShop.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly FreshnessService _freshnessService;
    private readonly DeliveryService _deliveryService;
    
    public AdminController(AppDbContext context, FreshnessService freshnessService, DeliveryService deliveryService)
    {
        _context = context;
        _freshnessService = freshnessService;
        _deliveryService = deliveryService;
    }
    
    // Product Management Endpoints
    
    [HttpPost("products")]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] CreateProductDto productDto)
    {
        var product = new Product
        {
            Name = productDto.Name,
            Price = productDto.Price,
            Stock = productDto.Stock,
            FreshnessScore = productDto.FreshnessScore,
            ImageUrl = productDto.ImageUrl,
            CategoryId = productDto.CategoryId,
            DefaultVaseLifeDays = productDto.DefaultVaseLifeDays
        };
        
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }
    
    [HttpGet("products/{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
            
        if (product == null)
        {
            return NotFound();
        }
        
        return product;
    }
    
    [HttpPut("products/{id}/stock")]
    public async Task<ActionResult> UpdateProductStock(int id, [FromBody] UpdateStockDto stockDto)
    {
        var product = await _context.Products.FindAsync(id);
        
        if (product == null)
        {
            return NotFound();
        }
        
        product.Stock = stockDto.Stock;
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpPut("products/{id}")]
    public async Task<ActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto productDto)
    {
        var product = await _context.Products.FindAsync(id);
        
        if (product == null)
        {
            return NotFound();
        }
        
        product.Name = productDto.Name;
        product.Price = productDto.Price;
        product.Stock = productDto.Stock;
        product.FreshnessScore = productDto.FreshnessScore;
        product.ImageUrl = productDto.ImageUrl;
        product.CategoryId = productDto.CategoryId;
        product.DefaultVaseLifeDays = productDto.DefaultVaseLifeDays;
        
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpDelete("products/{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        
        if (product == null)
        {
            return NotFound();
        }
        
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
    // Order Management Endpoints
    
    [HttpGet("orders")]
    public async Task<ActionResult<List<Order>>> GetAllOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.District)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
            
        return orders;
    }
    
    [HttpGet("orders/active")]
    public async Task<ActionResult<List<Order>>> GetActiveOrders()
    {
        var activeStatuses = new[] { "Hazırlanıyor", "Kuryede" };
        
        var orders = await _context.Orders
            .Include(o => o.District)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Where(o => activeStatuses.Contains(o.OrderStatus))
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
            
        return orders;
    }
    
    [HttpGet("orders/{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.District)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
            
        if (order == null)
        {
            return NotFound();
        }
        
        return order;
    }
    
    [HttpPut("orders/{id}/status")]
    public async Task<ActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto statusDto)
    {
        var order = await _context.Orders.FindAsync(id);
        
        if (order == null)
        {
            return NotFound();
        }
        
        order.OrderStatus = statusDto.Status;
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
    // Freshness Info Endpoint
    
    [HttpGet("products/{id}/freshness")]
    public ActionResult<FreshnessInfoDto> GetProductFreshness(int id, [FromQuery] DateTime? deliveryDate = null)
    {
        var product = _context.Products.Find(id);
        
        if (product == null)
        {
            return NotFound();
        }
        
        var freshnessInfo = _freshnessService.CalculateFreshness(product, deliveryDate);
        
        return freshnessInfo;
    }
    
    // Delivery Fee Calculation Endpoint
    
    [HttpGet("districts/{districtId}/delivery-fee")]
    public async Task<ActionResult<DeliveryInfoDto>> GetDeliveryFee(int districtId)
    {
        try
        {
            var deliveryInfo = await _deliveryService.CalculateDeliveryFeeAsync(districtId);
            return deliveryInfo;
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    // Wiki Note Management Endpoints
    
    [HttpPost("wiki-notes")]
    public async Task<ActionResult<WikiNote>> CreateWikiNote([FromBody] CreateWikiNoteDto wikiNoteDto)
    {
        var wikiNote = new WikiNote
        {
            Title = wikiNoteDto.Title,
            Content = wikiNoteDto.Content,
            Category = wikiNoteDto.Category
        };
        
        _context.WikiNotes.Add(wikiNote);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetWikiNote), new { id = wikiNote.Id }, wikiNote);
    }
    
    [HttpGet("wiki-notes/{id}")]
    public async Task<ActionResult<WikiNote>> GetWikiNote(int id)
    {
        var wikiNote = await _context.WikiNotes.FindAsync(id);
        
        if (wikiNote == null)
        {
            return NotFound();
        }
        
        return wikiNote;
    }
    
    [HttpPost("products/{productId}/wiki-notes/{wikiNoteId}")]
    public async Task<ActionResult> AddWikiNoteToProduct(int productId, int wikiNoteId)
    {
        var product = await _context.Products.Include(p => p.WikiNotes).FirstOrDefaultAsync(p => p.Id == productId);
        var wikiNote = await _context.WikiNotes.FindAsync(wikiNoteId);
        
        if (product == null || wikiNote == null)
        {
            return NotFound();
        }
        
        if (!product.WikiNotes.Contains(wikiNote))
        {
            product.WikiNotes.Add(wikiNote);
            await _context.SaveChangesAsync();
        }
        
        return NoContent();
    }
}
