namespace ShopForHome.API.DTOs
{
    public class CreateCouponDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal MinimumAmount { get; set; }
    }
}