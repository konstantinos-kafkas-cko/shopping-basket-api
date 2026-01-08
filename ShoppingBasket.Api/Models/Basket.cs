namespace ShoppingBasket.Api.Models;

public class Basket
{
    public string Username { get; set; } = null!;
    public Dictionary<string, BasketItem> Items { get; set; } = new();
    public HashSet<string> AppliedDiscountCodes { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string? ShippingRegion { get; set; }
    
    public decimal TotalDiscountedItemsPrice =>
        Items.Values
            .Where(i => i.IsDiscounted)
            .Sum(i => i.TotalPrice);

    public decimal TotalNonDiscountedItemsPrice =>
        Items.Values
            .Where(i => !i.IsDiscounted)
            .Sum(i => i.TotalPrice);
}