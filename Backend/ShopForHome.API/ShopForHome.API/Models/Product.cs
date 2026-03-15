namespace ShopForHome.API.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Rating { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Category? Category { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
        public ICollection<Wishlist>? Wishlists { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}