using FlowerShop.API.Data;
using FlowerShop.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        return product;
    }
}
