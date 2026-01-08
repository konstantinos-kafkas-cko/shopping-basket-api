using ShoppingBasket.Api.Models;

namespace ShoppingBasket.Api.Services;

public interface IBasketService
{
    public void AddItems(string username, List<AddItemRequest> requests);
    public bool RemoveItem(string username, string productId);
    public decimal GetTotal(string username, bool includeVat);
    public bool ApplyDiscountCode(string username, string discountCode);
    public bool SetShippingCountry(string username, string country);
    public IEnumerable<BasketItem> GetBasket(string username);
}