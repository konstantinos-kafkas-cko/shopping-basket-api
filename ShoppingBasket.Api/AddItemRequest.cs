using System.ComponentModel.DataAnnotations;

namespace ShoppingBasket.Api;

public class AddItemRequest
{
    [Required(ErrorMessage = "Product ID is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Product ID must be between 1 and 100 characters")]
    public string ProductId { get; set; } = null!;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999999, ErrorMessage = "Price must be between 0.01 and 999999")]
    public decimal Price { get; set; }

    [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
    public int Quantity { get; set; } = 1;

    public bool IsDiscounted { get; set; }
}