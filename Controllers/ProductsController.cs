using MediaInventoryManager.Data;
using MediaInventoryManager.DTOs;
using MediaInventoryManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediaInventoryManager.Controllers;

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
    public async Task<IActionResult> GetProducts(
    [FromQuery] int page = 1,
    [FromQuery] int limit = 10,
    [FromQuery] string? search = null)
    {
        if (page < 1)
            return BadRequest("Page must be greater than 0.");
        if (limit < 1 || limit > 100)
            return BadRequest("Limit must be between 1 and 100.");

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

        return Ok(new
        {
            data = products,
            totalPages,
            totalItems
        });
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

        return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
    }
}