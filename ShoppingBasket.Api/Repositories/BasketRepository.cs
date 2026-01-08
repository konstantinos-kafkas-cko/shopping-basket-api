using ShoppingBasket.Api.Models;

namespace ShoppingBasket.Api.Repositories;

public class BasketRepository : IBasketRepository
{
    private Dictionary<string, Basket> _basket = new();

    public Basket GetBasket(string username)
    {
        if (!_basket.ContainsKey(username))
        {
            _basket[username] = new Basket();
        }
        
        return _basket[username];
    }
    
    public bool RemoveItem(string username, string productId)
    {
        if (!_basket.ContainsKey(username))
            return false;
        
        var userBasket = _basket[username];
        if (!userBasket.Items.ContainsKey(productId))
            return false;
        
        return userBasket.Items.Remove(productId);
    }

    public void AddItem(BasketItem basketItem)
    {
        if (!_basket.ContainsKey(basketItem.Username))
        {
            _basket[basketItem.Username] = new Basket();
        }
        var userBasket = _basket[basketItem.Username];

        if (userBasket.Items.ContainsKey(basketItem.ProductId))
        {
            userBasket.Items[basketItem.ProductId].Quantity += basketItem.Quantity;
        }
        else
        {
            userBasket.Items[basketItem.ProductId] = basketItem;
        }
    }
}