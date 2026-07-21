using FlowerShop.API.Data;
using FlowerShop.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DistrictsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DistrictsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<District>>> GetDistricts()
    {
        return await _context.Districts.ToListAsync();
    }
}
