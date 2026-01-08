using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShoppingBasket.Api;
using ShoppingBasket.Api.Controllers;
using ShoppingBasket.Api.Services;

namespace ShoppingBasket.Api.Tests;

public class BasketControllerTests
{
    private readonly Mock<IBasketService> _mockService;
    private readonly BasketController _controller;

    public BasketControllerTests()
    {
        _mockService = new Mock<IBasketService>();
        _controller = new BasketController(_mockService.Object);
        var claims = new List<Claim> { new(ClaimTypes.Name, "test-user") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }

    [Fact]
    public void AddItems_ModelStateInvalid_ReturnsBadRequest()
    {
        _controller.ModelState.AddModelError("items", "Invalid");

        var result = _controller.AddItems(new List<AddItemRequest>());

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        _mockService.Verify(s => s.AddItems(It.IsAny<string>(), It.IsAny<List<AddItemRequest>>()), Times.Never);
    }

    [Fact]
    public void AddItems_ItemsNullOrEmpty_ReturnsBadRequest()
    {
        var resultNull = _controller.AddItems(null!);
        var resultEmpty = _controller.AddItems(new List<AddItemRequest>());

        Assert.IsType<BadRequestObjectResult>(resultNull);
        Assert.IsType<BadRequestObjectResult>(resultEmpty);
        _mockService.Verify(s => s.AddItems(It.IsAny<string>(), It.IsAny<List<AddItemRequest>>()), Times.Never);
    }

    [Fact]
    public void AddItems_ValidItems_CallsServiceAndReturnsOk()
    {
        var items = new List<AddItemRequest> { new() { ProductId = "p1", Price = 10m } };

        var result = _controller.AddItems(items);

        _mockService.Verify(s => s.AddItems("test-user", items), Times.Once);
        var ok = Assert.IsType<OkResult>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
    }

    [Fact]
    public void RemoveItem_ServiceReturnsFalse_ReturnsNotFound()
    {
        _mockService.Setup(s => s.RemoveItem("test-user", "p1")).Returns(false);

        var result = _controller.RemoveItem("p1");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);
        _mockService.Verify(s => s.RemoveItem("test-user", "p1"), Times.Once);
    }

    [Fact]
    public void RemoveItem_ServiceReturnsTrue_ReturnsOk()
    {
        _mockService.Setup(s => s.RemoveItem("test-user", "p1")).Returns(true);

        var result = _controller.RemoveItem("p1");

        var ok = Assert.IsType<OkResult>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        _mockService.Verify(s => s.RemoveItem("test-user", "p1"), Times.Once);
    }

    [Fact]
    public void GetTotal_ServiceReturnsValue_ReturnsOkWithValue()
    {
        _mockService.Setup(s => s.GetTotal("test-user", true)).Returns(42m);

        var result = _controller.GetTotal(includeVat: true);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        Assert.Equal(42m, ok.Value);
        _mockService.Verify(s => s.GetTotal("test-user", true), Times.Once);
    }

    [Fact]
    public void ApplyDiscountCode_ServiceReturnsFalse_ReturnsBadRequest()
    {
        _mockService.Setup(s => s.ApplyDiscountCode("test-user", "SUMMER10")).Returns(false);

        var result = _controller.ApplyDiscountCode("SUMMER10");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        _mockService.Verify(s => s.ApplyDiscountCode("test-user", "SUMMER10"), Times.Once);
    }

    [Fact]
    public void ApplyDiscountCode_ServiceReturnsTrue_ReturnsOk()
    {
        _mockService.Setup(s => s.ApplyDiscountCode("test-user", "SUMMER10")).Returns(true);

        var result = _controller.ApplyDiscountCode("SUMMER10");

        var ok = Assert.IsType<OkResult>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        _mockService.Verify(s => s.ApplyDiscountCode("test-user", "SUMMER10"), Times.Once);
    }

    [Fact]
    public void SetShipping_ServiceReturnsFalse_ReturnsBadRequest()
    {
        _mockService.Setup(s => s.SetShippingCountry("test-user", "UK")).Returns(false);

        var result = _controller.SetShipping("UK");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        _mockService.Verify(s => s.SetShippingCountry("test-user", "UK"), Times.Once);
    }

    [Fact]
    public void SetShipping_ServiceReturnsTrue_ReturnsOk()
    {
        _mockService.Setup(s => s.SetShippingCountry("test-user", "UK")).Returns(true);

        var result = _controller.SetShipping("UK");

        var ok = Assert.IsType<OkResult>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        _mockService.Verify(s => s.SetShippingCountry("test-user", "UK"), Times.Once);
    }
}