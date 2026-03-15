using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.API.Data;
using ShopForHome.API.DTOs;
using ShopForHome.API.Models;

namespace ShopForHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly ShopForHomeDbContext _context;

        public CouponsController(ShopForHomeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCoupons()
        {
            var coupons = await _context.Coupons
                .Select(c => new
                {
                    c.CouponId,
                    c.Code,
                    c.DiscountPercent,
                    c.ExpiryDate,
                    c.IsActive,
                    c.MinimumAmount
                })
                .ToListAsync();

            return Ok(coupons);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon(CreateCouponDto dto)
        {
            var exists = await _context.Coupons.AnyAsync(c => c.Code == dto.Code);

            if (exists)
                return BadRequest(new { message = "Coupon code already exists" });

            var coupon = new Coupon
            {
                Code = dto.Code,
                DiscountPercent = dto.DiscountPercent,
                ExpiryDate = dto.ExpiryDate,
                IsActive = true,
                MinimumAmount = dto.MinimumAmount
            };

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Coupon created successfully",
                data = new
                {
                    coupon.CouponId,
                    coupon.Code,
                    coupon.DiscountPercent,
                    coupon.ExpiryDate,
                    coupon.IsActive,
                    coupon.MinimumAmount
                }
            });
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignCouponToUser(AssignCouponDto dto)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == dto.UserId);
            if (!userExists)
                return NotFound(new { message = "User not found" });

            var couponExists = await _context.Coupons.AnyAsync(c => c.CouponId == dto.CouponId);
            if (!couponExists)
                return NotFound(new { message = "Coupon not found" });

            var alreadyAssigned = await _context.UserCoupons
                .AnyAsync(uc => uc.UserId == dto.UserId && uc.CouponId == dto.CouponId);

            if (alreadyAssigned)
                return BadRequest(new { message = "Coupon already assigned to this user" });

            var userCoupon = new UserCoupon
            {
                UserId = dto.UserId,
                CouponId = dto.CouponId,
                IsUsed = false,
                AssignedAt = DateTime.Now
            };

            _context.UserCoupons.Add(userCoupon);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Coupon assigned successfully",
                data = new
                {
                    userCoupon.UserCouponId,
                    userCoupon.UserId,
                    userCoupon.CouponId,
                    userCoupon.IsUsed,
                    userCoupon.AssignedAt
                }
            });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCouponsByUser(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
            if (!userExists)
                return NotFound(new { message = "User not found" });

            var userCoupons = await _context.UserCoupons
                .Include(uc => uc.Coupon)
                .Where(uc => uc.UserId == userId)
                .Select(uc => new
                {
                    uc.UserCouponId,
                    uc.UserId,
                    uc.CouponId,
                    CouponCode = uc.Coupon != null ? uc.Coupon.Code : null,
                    DiscountPercent = uc.Coupon != null ? uc.Coupon.DiscountPercent : 0,
                    ExpiryDate = uc.Coupon != null ? uc.Coupon.ExpiryDate : (DateTime?)null,
                    MinimumAmount = uc.Coupon != null ? uc.Coupon.MinimumAmount : 0,
                    uc.IsUsed,
                    uc.AssignedAt
                })
                .ToListAsync();

            return Ok(userCoupons);
        }
    }
}