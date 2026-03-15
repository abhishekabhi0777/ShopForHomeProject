namespace ShopForHome.API.Models;

public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public decimal TotalAmount { get; set; }
    public int? CouponId { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string Status { get; set; } = "Placed";

    public User? User { get; set; }
    public Coupon? Coupon { get; set; }
    public ICollection<OrderItem>? OrderItems { get; set; }
}