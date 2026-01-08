using ShoppingBasket.Api.Models;

namespace ShoppingBasket.Api.Repositories;

public interface IBasketRepository
{
    public void AddItem(BasketItem basketItem);
    public bool RemoveItem(string username, string productId);
    public Basket GetBasket(string username);
}