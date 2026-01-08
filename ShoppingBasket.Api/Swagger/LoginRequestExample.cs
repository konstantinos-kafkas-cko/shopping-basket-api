using ShoppingBasket.Api.Controllers;
using Swashbuckle.AspNetCore.Filters;

namespace ShoppingBasket.Api.Swagger;

public class LoginRequestExample : IExamplesProvider<LoginRequest>
{
    public LoginRequest GetExamples()
    {
        return new LoginRequest
        {
            Username = "admin",
            Password = "password"
        };
    }
}