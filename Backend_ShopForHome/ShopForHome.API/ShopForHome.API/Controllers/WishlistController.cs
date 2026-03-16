using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.API.Data;
using ShopForHome.API.DTOs;
using ShopForHome.API.Models;

namespace ShopForHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly ShopForHomeDbContext _context;

        public WishlistController(ShopForHomeDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWishlistByUserId(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

            if (!userExists)
                return NotFound(new { message = "User not found" });

            var wishlistItems = await _context.Wishlists
                .Include(w => w.Product)
                .Where(w => w.UserId == userId)
                .Select(w => new
                {
                    w.WishlistId,
                    w.UserId,
                    w.ProductId,
                    ProductName = w.Product != null ? w.Product.Name : null,
                    ProductDescription = w.Product != null ? w.Product.Description : null,
                    ProductPrice = w.Product != null ? w.Product.Price : 0,
                    ProductRating = w.Product != null ? w.Product.Rating : 0,
                    ProductImage = w.Product != null ? w.Product.ImageUrl : null,
                    w.AddedAt
                })
                .ToListAsync();

            return Ok(wishlistItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist(AddToWishlistDto dto)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == dto.UserId);
            if (!userExists)
                return NotFound(new { message = "User not found" });

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            var alreadyExists = await _context.Wishlists
                .AnyAsync(w => w.UserId == dto.UserId && w.ProductId == dto.ProductId);

            if (alreadyExists)
                return BadRequest(new { message = "Product already exists in wishlist" });

            var wishlist = new Wishlist
            {
                UserId = dto.UserId,
                ProductId = dto.ProductId,
                AddedAt = DateTime.Now
            };

            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Product added to wishlist successfully",
                data = new
                {
                    wishlist.WishlistId,
                    wishlist.UserId,
                    wishlist.ProductId,
                    wishlist.AddedAt
                }
            });
        }

        [HttpDelete("{wishlistId}")]
        public async Task<IActionResult> DeleteWishlistItem(int wishlistId)
        {
            var wishlistItem = await _context.Wishlists.FindAsync(wishlistId);

            if (wishlistItem == null)
                return NotFound(new { message = "Wishlist item not found" });

            _context.Wishlists.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Wishlist item deleted successfully" });
        }
    }
}