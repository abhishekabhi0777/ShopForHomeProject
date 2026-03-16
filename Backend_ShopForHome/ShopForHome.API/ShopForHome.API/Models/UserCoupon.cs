namespace ShopForHome.API.Models
{
    public class UserCoupon
    {
        public int UserCouponId { get; set; }
        public int UserId { get; set; }
        public int CouponId { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime AssignedAt { get; set; } = DateTime.Now;

        public User? User { get; set; }
        public Coupon? Coupon { get; set; }
    }
}