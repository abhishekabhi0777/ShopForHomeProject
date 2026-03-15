namespace ShopForHome.API.Models;

public class User
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Role { get; set; } = "User";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<CartItem>? CartItems { get; set; }
    public ICollection<Wishlist>? Wishlists { get; set; }
    public ICollection<Order>? Orders { get; set; }
    public ICollection<UserCoupon>?UserCoupons { get; set; }
}