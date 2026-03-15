using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.API.Data;

namespace ShopForHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ShopForHomeDbContext _context;

        public SearchController(ShopForHomeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> SearchProducts(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Keyword is required" });

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p =>
                    p.Name.Contains(keyword) ||
                    p.Description.Contains(keyword) ||
                    p.Category.Name.Contains(keyword))
                .Select(p => new
                {
                    p.ProductId,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.Rating,
                    p.Stock,
                    p.ImageUrl,
                    p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.Name : null,
                    p.CreatedAt
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterProducts(
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            decimal? minRating)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            if (minRating.HasValue)
                query = query.Where(p => p.Rating >= minRating.Value);

            var products = await query
                .Select(p => new
                {
                    p.ProductId,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.Rating,
                    p.Stock,
                    p.ImageUrl,
                    p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.Name : null,
                    p.CreatedAt
                })
                .ToListAsync();

            return Ok(products);
        }
    }
}