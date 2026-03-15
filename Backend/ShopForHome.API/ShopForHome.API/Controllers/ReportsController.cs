using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.API.Data;

namespace ShopForHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ShopForHomeDbContext _context;

        public ReportsController(ShopForHomeDbContext context)
        {
            _context = context;
        }

        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesReport(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(o => o.OrderDate.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(o => o.OrderDate.Date <= toDate.Value.Date);

            var orders = await query
                .Select(o => new
                {
                    o.OrderId,
                    o.UserId,
                    o.OrderDate,
                    o.TotalAmount,
                    o.DiscountAmount,
                    o.FinalAmount,
                    o.Status,
                    TotalItems = o.OrderItems != null ? o.OrderItems.Sum(i => i.Quantity) : 0
                })
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var totalOrders = orders.Count;
            var totalRevenue = orders.Sum(o => o.FinalAmount);
            var totalDiscount = orders.Sum(o => o.DiscountAmount);

            return Ok(new
            {
                totalOrders,
                totalRevenue,
                totalDiscount,
                orders
            });
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockProducts()
        {
            var lowStockProducts = await _context.Products
                .Where(p => p.Stock < 10)
                .Select(p => new
                {
                    p.ProductId,
                    p.Name,
                    p.Stock,
                    p.Price,
                    p.CategoryId
                })
                .ToListAsync();

            return Ok(lowStockProducts);
        }
    }
}