using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.API.Data;
using ShopForHome.API.Models;

namespace ShopForHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopForHomeDbContext _context;

        public ProductsController(ShopForHomeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.ProductId == id)
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
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(product);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
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

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.CategoryId == product.CategoryId);

            if (!categoryExists)
                return BadRequest(new { message = "Invalid CategoryId" });

            product.CreatedAt = DateTime.Now;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Product added successfully",
                data = new
                {
                    product.ProductId,
                    product.Name,
                    product.Description,
                    product.Price,
                    product.Rating,
                    product.Stock,
                    product.ImageUrl,
                    product.CategoryId,
                    product.CreatedAt
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.ProductId)
                return BadRequest(new { message = "Product ID mismatch" });

            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct == null)
                return NotFound(new { message = "Product not found" });

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.CategoryId == product.CategoryId);

            if (!categoryExists)
                return BadRequest(new { message = "Invalid CategoryId" });

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Rating = product.Rating;
            existingProduct.Stock = product.Stock;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.CategoryId = product.CategoryId;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Product updated successfully",
                data = new
                {
                    existingProduct.ProductId,
                    existingProduct.Name,
                    existingProduct.Description,
                    existingProduct.Price,
                    existingProduct.Rating,
                    existingProduct.Stock,
                    existingProduct.ImageUrl,
                    existingProduct.CategoryId,
                    existingProduct.CreatedAt
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound(new { message = "Product not found" });

            // Check if product is used in orders
            bool usedInOrders = _context.OrderItems.Any(o => o.ProductId == id);

            if (usedInOrders)
                return BadRequest(new { message = "Product cannot be deleted because it exists in orders" });

            // Remove wishlist entries
            var wishlistItems = _context.Wishlists.Where(w => w.ProductId == id);
            _context.Wishlists.RemoveRange(wishlistItems);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product deleted successfully" });
        }
    }
}