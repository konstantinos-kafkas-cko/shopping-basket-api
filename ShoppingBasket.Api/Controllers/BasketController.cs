using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingBasket.Api.Services;

namespace ShoppingBasket.Api.Controllers;

[ApiController]
[Route("api/basket")]
[Authorize]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;

    public BasketController(IBasketService basketService)
    {
        _basketService = basketService;
    }
    
    [HttpPost("items")]
    public IActionResult AddItems([FromBody] List<AddItemRequest> items)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (items is null || items.Count == 0)
        {
            return BadRequest("At least one item is required");
        }

        _basketService.AddItems(User.Identity!.Name!, items);
        return Ok();
    }
    
    [HttpDelete("items/{productId}")]
    public IActionResult RemoveItem(string productId)
    {
        var removed = _basketService.RemoveItem(User.Identity!.Name!, productId);
        if (!removed)
            return NotFound($"Product {productId} not found in basket.");

        return Ok();
    }

    [HttpGet("total")]
    public IActionResult GetTotal([FromQuery] bool includeVat = true)
    {
        var total = _basketService.GetTotal(User.Identity!.Name!, includeVat);
        return Ok(total);
    }
    
    [HttpPost("discount-code")]
    public IActionResult ApplyDiscountCode([FromQuery] string code)
    {
        var success = _basketService.ApplyDiscountCode(User.Identity!.Name!, code);
        if (!success)
            return BadRequest($"Discount code '{code}' is invalid or expired.");

        return Ok();
    }

    [HttpPost("shipping")]
    public IActionResult SetShipping([FromQuery] string country)
    {
        var success = _basketService.SetShippingCountry(User.Identity!.Name!, country);
        if (!success)
            return BadRequest($"Shipping country '{country}' not supported.");
        
        return Ok();
    }
    
    [HttpGet("items")]
    public IActionResult GetBasket()
    {
        var basket = _basketService.GetBasket(User.Identity!.Name!);
        if (!basket.Any()) 
            return NotFound($"Empty basket.");
        
        return Ok(basket);
    }
}