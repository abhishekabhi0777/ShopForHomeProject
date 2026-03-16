using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.API.Data;
using ShopForHome.API.DTOs;
using ShopForHome.API.Models;

namespace ShopForHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ShopForHomeDbContext _context;

        public UsersController(ShopForHomeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.UserId,
                    u.FullName,
                    u.Email,
                    u.Phone,
                    u.Address,
                    u.Role,
                    u.IsActive,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users
                .Where(u => u.UserId == id)
                .Select(u => new
                {
                    u.UserId,
                    u.FullName,
                    u.Email,
                    u.Phone,
                    u.Address,
                    u.Role,
                    u.IsActive,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound(new { message = "User not found" });

            if (await _context.Users.AnyAsync(x => x.Email == dto.Email && x.UserId != id))
                return BadRequest(new { message = "Email already exists" });

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Phone = dto.Phone;
            user.Address = dto.Address;
            user.Role = dto.Role;
            user.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return Ok(new { message = "User updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                    return NotFound(new { message = "User not found" });

                var cartItems = _context.CartItems.Where(c => c.UserId == id);
                _context.CartItems.RemoveRange(cartItems);

                var wishlists = _context.Wishlists.Where(w => w.UserId == id);
                _context.Wishlists.RemoveRange(wishlists);

                var userCoupons = _context.UserCoupons.Where(uc => uc.UserId == id);
                _context.UserCoupons.RemoveRange(userCoupons);

                var orders = _context.Orders.Where(o => o.UserId == id).ToList();

                foreach (var order in orders)
                {
                    var orderItems = _context.OrderItems.Where(oi => oi.OrderId == order.OrderId);
                    _context.OrderItems.RemoveRange(orderItems);
                }

                _context.Orders.RemoveRange(orders);
                _context.Users.Remove(user);

                await _context.SaveChangesAsync();

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error while deleting user",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
    }
}