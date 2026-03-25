using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.API.Data;
using ShopForHome.API.DTOs;
using ShopForHome.API.Models;
using System;

namespace ShopForHome.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ShopForHomeDbContext _context;

        public OrdersController(ShopForHomeDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            if(dto.Items == null || !dto.Items.Any())
    return BadRequest(new { message = "Cart is empty" });

            decimal totalAmount = dto.Items.Sum(i => i.Quantity * i.UnitPrice);
            decimal discountAmount = 0;
            int? appliedCouponId = null;

            if (dto.CouponId.HasValue)
            {
                var userCoupon = await _context.UserCoupons
                    .Include(uc => uc.Coupon)
                    .FirstOrDefaultAsync(uc =>
                        uc.UserId == dto.UserId &&
                        uc.CouponId == dto.CouponId.Value &&
                        !uc.IsUsed);

                if (userCoupon == null || userCoupon.Coupon == null)
                    return BadRequest(new { message = "Invalid coupon" });

                if (!userCoupon.Coupon.IsActive)
                    return BadRequest(new { message = "Coupon is inactive" });

                if (userCoupon.Coupon.ExpiryDate < DateTime.Now)
                    return BadRequest(new { message = "Coupon expired" });

                if (totalAmount < userCoupon.Coupon.MinimumAmount)
                    return BadRequest(new { message = "Minimum amount not reached for coupon" });

                discountAmount = totalAmount * (userCoupon.Coupon.DiscountPercent / 100m);
                appliedCouponId = userCoupon.CouponId;

                userCoupon.IsUsed = true;
            }

            decimal finalAmount = totalAmount - discountAmount;

            var order = new Order
            {
                UserId = dto.UserId,
                CouponId = appliedCouponId,
                TotalAmount = totalAmount,
                DiscountAmount = discountAmount,
                FinalAmount = finalAmount,
                OrderDate = DateTime.Now,
                Status = "Placed"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Order placed successfully",
                orderId = order.OrderId,
                totalAmount,
                discountAmount,
                finalAmount
            });
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUser(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    o.TotalAmount,
                    o.DiscountAmount,
                    o.FinalAmount,
                    o.Status,

                    Items = _context.OrderItems
                        .Where(i => i.OrderId == o.OrderId)
                        .Join(_context.Products,
                            i => i.ProductId,
                            p => p.ProductId,
                            (i, p) => new
                            {
                                i.ProductId,
                                i.Quantity,
                                price = i.UnitPrice,
                                productName = p.Name,
                                imageUrl = p.ImageUrl
                            })
                        .ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }
    }
}