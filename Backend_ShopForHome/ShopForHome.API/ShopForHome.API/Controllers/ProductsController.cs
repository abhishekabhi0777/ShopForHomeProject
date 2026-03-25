using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.API.Data;
using ShopForHome.API.Models;
using System.Globalization;

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

        [HttpPost("upload-csv")]
        public async Task<IActionResult> UploadProductsCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "CSV file is required" });

            var productsToAdd = new List<Product>();
            var errors = new List<string>();

            using var stream = new StreamReader(file.OpenReadStream());
            string? headerLine = await stream.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(headerLine))
                return BadRequest(new { message = "CSV file is empty" });

            int rowNumber = 1;

            while (!stream.EndOfStream)
            {
                rowNumber++;
                var line = await stream.ReadLineAsync();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var values = line.Split(',');

                if (values.Length < 7)
                {
                    errors.Add($"Row {rowNumber}: Invalid column count");
                    continue;
                }

                try
                {
                    var name = values[0].Trim();
                    var description = values[1].Trim();
                    var price = decimal.Parse(values[2].Trim(), CultureInfo.InvariantCulture);
                    var rating = decimal.Parse(values[3].Trim(), CultureInfo.InvariantCulture);
                    var stock = int.Parse(values[4].Trim());
                    var imageUrl = values[5].Trim();
                    var categoryId = int.Parse(values[6].Trim());

                    var categoryExists = await _context.Categories
                        .AnyAsync(c => c.CategoryId == categoryId);

                    if (!categoryExists)
                    {
                        errors.Add($"Row {rowNumber}: Invalid CategoryId {categoryId}");
                        continue;
                    }

                    productsToAdd.Add(new Product
                    {
                        Name = name,
                        Description = description,
                        Price = price,
                        Rating = rating,
                        Stock = stock,
                        ImageUrl = imageUrl,
                        CategoryId = categoryId,
                        CreatedAt = DateTime.Now
                    });
                }
                catch
                {
                    errors.Add($"Row {rowNumber}: Invalid data format");
                }
            }

            if (productsToAdd.Count > 0)
            {
                _context.Products.AddRange(productsToAdd);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                message = "CSV upload completed",
                insertedCount = productsToAdd.Count,
                errorCount = errors.Count,
                errors
            });
        }
    }
}