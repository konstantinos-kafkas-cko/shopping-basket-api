using Swashbuckle.AspNetCore.Filters;

namespace ShoppingBasket.Api.Swagger;

public class AddItemRequestExample : IExamplesProvider<List<AddItemRequest>>
{
    public List<AddItemRequest> GetExamples()
    {
        return new List<AddItemRequest>
        {
            new() { ProductId = "p1", Price = 10m, Quantity = 1, IsDiscounted = false },
            new() { ProductId = "p2", Price = 5m, Quantity = 2, IsDiscounted = true }
        };
    }
}