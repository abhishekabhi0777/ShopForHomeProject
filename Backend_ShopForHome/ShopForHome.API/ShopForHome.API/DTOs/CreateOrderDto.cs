namespace ShopForHome.API.DTOs
{
    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public int? CouponId { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}