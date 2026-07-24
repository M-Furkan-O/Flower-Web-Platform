using FlowerShop.API.Data;
using FlowerShop.API.DTOs;
using FlowerShop.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FlowerShop.API.Services;

public class WikiService
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions _cacheOptions;
    
    public WikiService(AppDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
        _cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
            SlidingExpiration = TimeSpan.FromMinutes(30)
        };
    }
    
    public async Task<List<WikiNoteDto>> GetWikiNotesByProductIdAsync(int productId)
    {
        var cacheKey = $"wiki_product_{productId}";
        var cachedData = await _cache.GetStringAsync(cacheKey);
        
        if (cachedData != null)
        {
            return JsonSerializer.Deserialize<List<WikiNoteDto>>(cachedData) ?? new List<WikiNoteDto>();
        }
        
        var product = await _context.Products
            .Include(p => p.WikiNotes)
            .FirstOrDefaultAsync(p => p.Id == productId);
            
        if (product == null)
        {
            throw new ArgumentException("Product not found");
        }
        
        var result = product.WikiNotes
            .Select(wn => new WikiNoteDto
            {
                Id = wn.Id,
                Title = wn.Title,
                Content = wn.Content,
                Category = wn.Category
            })
            .ToList();
            
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), _cacheOptions);
        
        return result;
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
        var cacheKey = "wiki_all";
        var cachedData = await _cache.GetStringAsync(cacheKey);
        
        if (cachedData != null)
        {
            return JsonSerializer.Deserialize<List<WikiNoteDto>>(cachedData) ?? new List<WikiNoteDto>();
        }
        
        var wikiNotes = await _context.WikiNotes.ToListAsync();
        
        var result = wikiNotes
            .Select(wn => new WikiNoteDto
            {
                Id = wn.Id,
                Title = wn.Title,
                Content = wn.Content,
                Category = wn.Category
            })
            .ToList();
            
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), _cacheOptions);
        
        return result;
    }
    
    public async Task InvalidateWikiCacheAsync(int? productId = null)
    {
        if (productId.HasValue)
        {
            await _cache.RemoveAsync($"wiki_product_{productId}");
        }
        await _cache.RemoveAsync("wiki_all");
    }
}
