using Moq;
using ShoppingBasket.Api;
using ShoppingBasket.Api.Models;
using ShoppingBasket.Api.Options;
using ShoppingBasket.Api.Repositories;
using ShoppingBasket.Api.Services;

namespace ShoppingBasket.Api.Tests;

public class BasketServiceTests
{
    private readonly Mock<IBasketRepository> _mockRepository;
    private readonly BasketService _service;

    public BasketServiceTests()
    {
        _mockRepository = new Mock<IBasketRepository>();
        var discountOptions = new DiscountOptions
        {
            ["SUMMER10"] = 0.10m
        };
        var shippingOptions = new ShippingOptions
        {
            ["UK"] = 5m,
            ["Other"] = 15m
        };
        var vatOptions = new VatOptions { Vat = 0.20m };
        _service = new BasketService(
            _mockRepository.Object,
            Microsoft.Extensions.Options.Options.Create(discountOptions),
            Microsoft.Extensions.Options.Options.Create(shippingOptions),
            Microsoft.Extensions.Options.Options.Create(vatOptions)
        );
    }

    [Fact]
    public void AddItems_ArgumentsInvalid_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.AddItems("", new List<AddItemRequest> { new() { ProductId = "p1", Price = 10m } }));
        Assert.Throws<ArgumentException>(() => _service.AddItems("user1", null!));
        Assert.Throws<ArgumentException>(() => _service.AddItems("user1", new List<AddItemRequest>()));
        
        _mockRepository.Verify(r => r.AddItem(It.IsAny<BasketItem>()), Times.Never);
    }
    
    [Fact]
    public void AddItems_ValidRequests_AddsItemsToRepository()
    {
        var requests = new List<AddItemRequest>
        {
            new() { ProductId = "p1", Price = 10m, Quantity = 2, IsDiscounted = false },
            new() { ProductId = "p2", Price = 5m, Quantity = 1, IsDiscounted = true }
        };

        _service.AddItems("user1", requests);

        _mockRepository.Verify(r => r.AddItem(It.Is<BasketItem>(item =>
            item.Username == "user1" &&
            item.ProductId == "p1" &&
            item.Price == 10m &&
            item.Quantity == 2 &&
            item.IsDiscounted == false)), Times.Once);

        _mockRepository.Verify(r => r.AddItem(It.Is<BasketItem>(item =>
            item.Username == "user1" &&
            item.ProductId == "p2" &&
            item.Price == 5m &&
            item.Quantity == 1 &&
            item.IsDiscounted == true)), Times.Once);
    }

    [Fact]
    public void RemoveItem_ArgumentsInvalid_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _service.RemoveItem("", "p1"));
        Assert.Throws<ArgumentException>(() => _service.RemoveItem("user1", ""));

        _mockRepository.Verify(r => r.RemoveItem(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void RemoveItem_ItemExists_ReturnsTrue()
    {
        _mockRepository.Setup(r => r.RemoveItem("user1", "p1")).Returns(true);

        var result = _service.RemoveItem("user1", "p1");

        Assert.True(result);
        _mockRepository.Verify(r => r.RemoveItem("user1", "p1"), Times.Once);
    }

    [Fact]
    public void ApplyDiscountCode_CodeNotConfigured_ReturnsFalse()
    {
        var service = new BasketService(
            _mockRepository.Object,
            Microsoft.Extensions.Options.Options.Create(new DiscountOptions()),
            Microsoft.Extensions.Options.Options.Create(new ShippingOptions()),
            Microsoft.Extensions.Options.Options.Create(new VatOptions())
        );

        var result = service.ApplyDiscountCode("user1", "UNKNOWN");

        Assert.False(result);
        _mockRepository.Verify(r => r.GetBasket(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void ApplyDiscountCode_CodeAlreadyApplied_ReturnsFalse()
    {
        var basket = new Basket { AppliedDiscountCodes = new HashSet<string> { "SUMMER10" } };
        _mockRepository.Setup(r => r.GetBasket("user1")).Returns(basket);

        var result = _service.ApplyDiscountCode("user1", "SUMMER10");

        Assert.False(result);
        _mockRepository.Verify(r => r.GetBasket("user1"), Times.Once);
    }

    [Fact]
    public void ApplyDiscountCode_ValidAndNotApplied_AddsCode()
    {
        var basket = new Basket { AppliedDiscountCodes = new HashSet<string>() };
        _mockRepository.Setup(r => r.GetBasket("user1")).Returns(basket);

        var result = _service.ApplyDiscountCode("user1", "SUMMER10");

        Assert.True(result);
        Assert.Contains("SUMMER10", basket.AppliedDiscountCodes);
        _mockRepository.Verify(r => r.GetBasket("user1"), Times.Once);
    }

    [Fact]
    public void SetShippingCountry_CountryNotConfigured_ReturnsFalse()
    {
        var service = new BasketService(
            _mockRepository.Object,
            Microsoft.Extensions.Options.Options.Create(new DiscountOptions()),
            Microsoft.Extensions.Options.Options.Create(new ShippingOptions()),
            Microsoft.Extensions.Options.Options.Create(new VatOptions())
        );

        var result = service.SetShippingCountry("user1", "UK");

        Assert.False(result);
        _mockRepository.Verify(r => r.GetBasket(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void SetShippingCountry_SupportedCountry_UpdatesBasket()
    {
        var basket = new Basket();
        _mockRepository.Setup(r => r.GetBasket("user1")).Returns(basket);

        var result = _service.SetShippingCountry("user1", "UK");

        Assert.True(result);
        Assert.Equal("UK", basket.ShippingRegion);
        _mockRepository.Verify(r => r.GetBasket("user1"), Times.Once);
    }

    [Fact]
    public void GetTotal_WithDiscountShippingAndVat_CalculatesTotal()
    {
        var basket = new Basket
        {
            Items = new Dictionary<string, BasketItem>
            {
                ["p1"] = new() { Username = "user1", ProductId = "p1", Price = 100m, Quantity = 1 }
            },
            AppliedDiscountCodes = new HashSet<string> { "SUMMER10" },
            ShippingRegion = "UK"
        };
        _mockRepository.Setup(r => r.GetBasket("user1")).Returns(basket);

        var total = _service.GetTotal("user1", includeVat: true);

        // 100 - 10% discount => 90 + 5 shipping => 95 + 20% VAT => 114
        Assert.Equal(114m, total);
        _mockRepository.Verify(r => r.GetBasket("user1"), Times.Once);
    }

    [Fact]
    public void GetTotal_IncludeVatFalse_DoesNotApplyVat()
    {
        var basket = new Basket
        {
            Items = new Dictionary<string, BasketItem>
            {
                ["p1"] = new() { Username = "user1", ProductId = "p1", Price = 100m, Quantity = 1 }
            },
            ShippingRegion = "UK"
        };
        _mockRepository.Setup(r => r.GetBasket("user1")).Returns(basket);

        var total = _service.GetTotal("user1", includeVat: false);

        // 100 + 5 shipping = 105
        Assert.Equal(105m, total);
        _mockRepository.Verify(r => r.GetBasket("user1"), Times.Once);
    }

    [Fact]
    public void GetTotal_UsernameIsEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _service.GetTotal("", includeVat: true));

        _mockRepository.Verify(r => r.GetBasket(It.IsAny<string>()), Times.Never);
    }
}
