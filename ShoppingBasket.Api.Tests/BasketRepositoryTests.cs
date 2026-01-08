using ShoppingBasket.Api.Models;
using ShoppingBasket.Api.Repositories;

namespace ShoppingBasket.Api.Tests;

public class BasketRepositoryTests
{
    [Fact]
    public void AddItem_ItemNotInBasket_AddsNewItem()
    {
        var repository = new BasketRepository();
        var item = new BasketItem
        {
            Username = "user1",
            ProductId = "p1",
            Price = 10m,
            Quantity = 1,
            IsDiscounted = false
        };

        repository.AddItem(item);

        var basket = repository.GetBasket("user1");
        Assert.True(basket.Items.ContainsKey("p1"));
        Assert.Equal(1, basket.Items["p1"].Quantity);
    }

    [Fact]
    public void AddItem_ItemAlreadyInBasket_IncreasesQuantity()
    {
        var repository = new BasketRepository();
        var item1 = new BasketItem
        {
            Username = "user1",
            ProductId = "p1",
            Price = 10m,
            Quantity = 1,
            IsDiscounted = false
        };
        var item2 = new BasketItem
        {
            Username = "user1",
            ProductId = "p1",
            Price = 10m,
            Quantity = 2,
            IsDiscounted = false
        };

        repository.AddItem(item1);
        repository.AddItem(item2);

        var basket = repository.GetBasket("user1");
        Assert.True(basket.Items.ContainsKey("p1"));
        Assert.Equal(3, basket.Items["p1"].Quantity);
    }

    [Fact]
    public void RemoveItem_UserDoesNotExist_ReturnsFalse()
    {
        var repository = new BasketRepository();

        var result = repository.RemoveItem("user1", "p1");

        Assert.False(result);
    }
    
    [Fact]
    public void RemoveItem_ItemDoesNotExist_ReturnsFalse()
    {
        var repository = new BasketRepository();
        var item = new BasketItem
        {
            Username = "user1",
            ProductId = "p1",
            Price = 10m,
            Quantity = 1,
            IsDiscounted = false
        };
        repository.AddItem(item);

        var result = repository.RemoveItem("user1", "p2");

        Assert.False(result);
    }
    
    [Fact]
    public void RemoveItem_ItemExists_RemovesItemAndReturnsTrue()
    {
        var repository = new BasketRepository();
        var item = new BasketItem
        {
            Username = "user1",
            ProductId = "p1",
            Price = 10m,
            Quantity = 1,
            IsDiscounted = false
        };
        repository.AddItem(item);

        var result = repository.RemoveItem("user1", "p1");

        Assert.True(result);
        var basket = repository.GetBasket("user1");
        Assert.False(basket.Items.ContainsKey("p1"));
    }
}


