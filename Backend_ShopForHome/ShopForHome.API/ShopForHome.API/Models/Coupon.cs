namespace ShopForHome.API.Models
{
    public class Coupon
    {
        public int CouponId { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
        public decimal MinimumAmount { get; set; }

        public ICollection<UserCoupon>? UserCoupons { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}