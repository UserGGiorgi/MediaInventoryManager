using MediaInventoryManager.Data;
using MediaInventoryManager.DTOs;
using MediaInventoryManager.Models;
using MediaInventoryManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace MediaInventoryManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ProductCacheService _cacheService;

    public ProductsController(
        AppDbContext context,
        IMemoryCache cache,
        ProductCacheService cacheService)
    {
        _context = context;
        _cache = cache;
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null)
    {
        if (page < 1)
            return BadRequest("Page must be greater than 0.");
        if (limit < 1 || limit > 100)
            return BadRequest("Limit must be between 1 and 100.");

        string cacheKey = $"product_list_{page}_{limit}_{search ?? "all"}";

        if (_cache.TryGetValue(cacheKey, out object? cachedResult))
            return Ok(cachedResult);

        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string searchLower = search.ToLower();
            query = query.Where(p => p.Title.ToLower().Contains(searchLower));
        }

        int totalItems = await query.CountAsync();

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        int totalPages = (int)Math.Ceiling(totalItems / (double)limit);

        var response = new
        {
            data = products,
            totalPages,
            totalItems
        };

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
            .AddExpirationToken(new CancellationChangeToken(_cacheService.GetCurrentToken()));

        _cache.Set(cacheKey, response, cacheEntryOptions);

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = new Product
        {
            Title = dto.Title,
            Description = dto.Description,
            ImageFileName = dto.ImageFileName,
            Price = dto.Price,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _cacheService.Invalidate();

        return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
    }
}