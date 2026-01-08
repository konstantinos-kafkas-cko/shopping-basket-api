namespace ShoppingBasket.Api.Models;

public class BasketItem
{
    public string Username { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
    public bool IsDiscounted { get; set; }

    public decimal TotalPrice => Price * Quantity;
}