using FlowerShop.API.DTOs;
using FlowerShop.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WikiController : ControllerBase
{
    private readonly WikiService _wikiService;
    
    public WikiController(WikiService wikiService)
    {
        _wikiService = wikiService;
    }
    
    [HttpGet("products/{productId}")]
    public async Task<ActionResult<List<WikiNoteDto>>> GetWikiNotesByProduct(int productId)
    {
        try
        {
            var wikiNotes = await _wikiService.GetWikiNotesByProductIdAsync(productId);
            return wikiNotes;
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [HttpGet("products/{productId}/grouped")]
    public async Task<ActionResult<Dictionary<string, List<WikiNoteDto>>>> GetWikiNotesGroupedByCategory(int productId)
    {
        try
        {
            var wikiNotes = await _wikiService.GetWikiNotesGroupedByCategoryAsync(productId);
            return wikiNotes;
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [HttpGet("category/{category}")]
    public async Task<ActionResult<List<WikiNoteDto>>> GetWikiNotesByCategory(string category)
    {
        var wikiNotes = await _wikiService.GetWikiNotesByCategoryAsync(category);
        return wikiNotes;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<WikiNoteDto>>> GetAllWikiNotes()
    {
        var wikiNotes = await _wikiService.GetAllWikiNotesAsync();
        return wikiNotes;
    }
}
