using Microsoft.Extensions.Options;
using ShoppingBasket.Api.Models;
using ShoppingBasket.Api.Options;
using ShoppingBasket.Api.Repositories;

namespace ShoppingBasket.Api.Services;

public class BasketService: IBasketService
{
    private readonly IBasketRepository _basketRepository;
    private readonly DiscountOptions _discountOptions;
    private readonly ShippingOptions _shippingOptions;
    private readonly VatOptions _vatOptions;
    
    public BasketService(
        IBasketRepository basketRepository, 
        IOptions<DiscountOptions> discountOptions,
        IOptions<ShippingOptions> shippingOptions,
        IOptions<VatOptions> vatOptions)
    {
        _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
        _discountOptions = discountOptions.Value ?? throw new ArgumentNullException(nameof(discountOptions));
        _shippingOptions = shippingOptions.Value ?? throw new ArgumentNullException(nameof(shippingOptions));
        _vatOptions = vatOptions.Value ?? throw new ArgumentNullException(nameof(vatOptions));
    }
    
    public void AddItems(string username, List<AddItemRequest> requests)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required", nameof(username));
        
        if (requests == null || requests.Count == 0)
            throw new ArgumentException("Items are required", nameof(requests));
        
        foreach (var request in requests)
        {
            var basketItem = new BasketItem()
            {
                Username = username,
                ProductId = request.ProductId,
                Price = request.Price,
                Quantity = request.Quantity,
                IsDiscounted = request.IsDiscounted
            };
            _basketRepository.AddItem(basketItem);
        }
    }

    public bool RemoveItem(string username, string productId)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required", nameof(username));
        
        if (string.IsNullOrEmpty(productId))
            throw new ArgumentException("Product id is required", nameof(productId));
        
        return _basketRepository.RemoveItem(username, productId);
    }
    
    public bool ApplyDiscountCode(string username, string discountCode)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required", nameof(username));
        
        if (string.IsNullOrWhiteSpace(discountCode))
            throw new ArgumentException("discount code is required", nameof(discountCode));

        if (!_discountOptions.ContainsKey(discountCode)) return false;

        var basket = _basketRepository.GetBasket(username);
        
        if (!basket.AppliedDiscountCodes.Add(discountCode))
            return false;

        return true;
    }

    public bool SetShippingCountry(string username, string country)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required", nameof(username));
        
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country is required", nameof(country));
        
        if (!_shippingOptions.ContainsKey(country)) return false;
        
        var basket = _basketRepository.GetBasket(username);
        
        basket.ShippingRegion = country;
        
        return true;
    }
    
    public decimal GetTotal(string username, bool includeVat)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required", nameof(username));

        var basket = _basketRepository.GetBasket(username);

        decimal total = CalculateNonDiscountedTotal(basket);
        total += CalculateShippingCost(basket);

        if (includeVat) total = ApplyVat(total);

        return total;
    }

    private decimal CalculateNonDiscountedTotal(Basket basket)
    {
        decimal nonDiscountedTotal = basket.TotalNonDiscountedItemsPrice;
        foreach (var code in basket.AppliedDiscountCodes)
        {
            var percentage = _discountOptions[code];
            nonDiscountedTotal -= nonDiscountedTotal * percentage;
        }

        return nonDiscountedTotal + basket.TotalDiscountedItemsPrice;
    }

    private decimal CalculateShippingCost(Basket basket)
    {
        if (basket.ShippingRegion is null)
            return 0m;

        if (_shippingOptions.TryGetValue(basket.ShippingRegion, out var shippingCost))
            return shippingCost;

        return 0m;
    }

    private decimal ApplyVat(decimal amount)
    {
        return amount + (amount * _vatOptions.Vat);
    }
    
    public IEnumerable<BasketItem> GetBasket(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required", nameof(username));

        return _basketRepository.GetBasket(username).Items.Values;
    }
}