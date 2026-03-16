namespace ShopForHome.API.Models;

public class Wishlist
{
    public int WishlistId { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.Now;

    public User? User { get; set; }
    public Product? Product { get; set; }
}