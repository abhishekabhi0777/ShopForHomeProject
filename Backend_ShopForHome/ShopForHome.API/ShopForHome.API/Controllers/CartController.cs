using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.API.Data;
using ShopForHome.API.DTOs;
using ShopForHome.API.Models;

namespace ShopForHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ShopForHomeDbContext _context;

        public CartController(ShopForHomeDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCartByUserId(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

            if (!userExists)
                return NotFound(new { message = "User not found" });

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .Select(c => new
                {
                    c.CartItemId,
                    c.UserId,
                    c.ProductId,
                    ProductName = c.Product != null ? c.Product.Name : null,
                    ProductPrice = c.Product != null ? c.Product.Price : 0,
                    ProductImage = c.Product != null ? c.Product.ImageUrl : null,
                    c.Quantity,
                    TotalPrice = c.Product != null ? c.Product.Price * c.Quantity : 0
                })
                .ToListAsync();

            return Ok(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(AddToCartDto dto)
        {
            if (dto.Quantity <= 0)
                return BadRequest(new { message = "Quantity must be greater than 0" });

            var userExists = await _context.Users.AnyAsync(u => u.UserId == dto.UserId);
            if (!userExists)
                return NotFound(new { message = "User not found" });

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == dto.UserId && c.ProductId == dto.ProductId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += dto.Quantity;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Cart quantity updated successfully",
                    data = new
                    {
                        existingCartItem.CartItemId,
                        existingCartItem.UserId,
                        existingCartItem.ProductId,
                        existingCartItem.Quantity
                    }
                });
            }

            var cartItem = new CartItem
            {
                UserId = dto.UserId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Product added to cart successfully",
                data = new
                {
                    cartItem.CartItemId,
                    cartItem.UserId,
                    cartItem.ProductId,
                    cartItem.Quantity
                }
            });
        }

        [HttpPut("{cartItemId}")]
        public async Task<IActionResult> UpdateCartQuantity(int cartItemId, UpdateCartDto dto)
        {
            if (dto.Quantity <= 0)
                return BadRequest(new { message = "Quantity must be greater than 0" });

            var cartItem = await _context.CartItems.FindAsync(cartItemId);

            if (cartItem == null)
                return NotFound(new { message = "Cart item not found" });

            cartItem.Quantity = dto.Quantity;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cart updated successfully",
                data = new
                {
                    cartItem.CartItemId,
                    cartItem.UserId,
                    cartItem.ProductId,
                    cartItem.Quantity
                }
            });
        }

        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> DeleteCartItem(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);

            if (cartItem == null)
                return NotFound(new { message = "Cart item not found" });

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cart item deleted successfully" });
        }
    }
}