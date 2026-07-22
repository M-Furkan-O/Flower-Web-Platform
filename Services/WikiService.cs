using FlowerShop.API.Data;
using FlowerShop.API.DTOs;
using FlowerShop.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.API.Services;

public class WikiService
{
    private readonly AppDbContext _context;
    
    public WikiService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<WikiNoteDto>> GetWikiNotesByProductIdAsync(int productId)
    {
        var product = await _context.Products
            .Include(p => p.WikiNotes)
            .FirstOrDefaultAsync(p => p.Id == productId);
            
        if (product == null)
        {
            throw new ArgumentException("Product not found");
        }
        
        return product.WikiNotes
            .Select(wn => new WikiNoteDto
            {
                Id = wn.Id,
                Title = wn.Title,
                Content = wn.Content,
                Category = wn.Category
            })
            .ToList();
    }
    
    public async Task<List<WikiNoteDto>> GetWikiNotesByCategoryAsync(string category)
    {
        var wikiNotes = await _context.WikiNotes
            .Where(wn => wn.Category == category)
            .ToListAsync();
            
        return wikiNotes
            .Select(wn => new WikiNoteDto
            {
                Id = wn.Id,
                Title = wn.Title,
                Content = wn.Content,
                Category = wn.Category
            })
            .ToList();
    }
    
    public async Task<Dictionary<string, List<WikiNoteDto>>> GetWikiNotesGroupedByCategoryAsync(int productId)
    {
        var product = await _context.Products
            .Include(p => p.WikiNotes)
            .FirstOrDefaultAsync(p => p.Id == productId);
            
        if (product == null)
        {
            throw new ArgumentException("Product not found");
        }
        
        return product.WikiNotes
            .GroupBy(wn => wn.Category)
            .ToDictionary(
                g => g.Key,
                g => g.Select(wn => new WikiNoteDto
                {
                    Id = wn.Id,
                    Title = wn.Title,
                    Content = wn.Content,
                    Category = wn.Category
                }).ToList()
            );
    }
    
    public async Task<List<WikiNoteDto>> GetAllWikiNotesAsync()
    {
        var wikiNotes = await _context.WikiNotes.ToListAsync();
        
        return wikiNotes
            .Select(wn => new WikiNoteDto
            {
                Id = wn.Id,
                Title = wn.Title,
                Content = wn.Content,
                Category = wn.Category
            })
            .ToList();
    }
}
