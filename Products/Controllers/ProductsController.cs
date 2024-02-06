using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Auth;
using Products.Model;
using System.Net;

namespace Products.Controllers
{

    //[Authorize(Roles = UserRoles.User)]
    //[Route("api/[controller]")]
    //[ApiController]    
    public class ProductsController : ControllerBase
    {
        public static readonly List<Product> ProductsList = new List<Product>()
        {
            new Product { Id = 1, Name = "Carpet", Colour = "Red" },
            new Product { Id = 2, Name = "Vase", Colour = "Blue" },
            new Product { Id = 3, Name = "Blanket", Colour = "Yellow" },
        };


        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ILogger<ProductsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("healthCheck")]
        public HttpStatusCode HealthCheck()
        {
            return HttpStatusCode.OK;
        }

        [HttpGet]
        [Route("api/products")]
        [Authorize]
        public IList<Product> GetProducts()
        {
            return ProductsList;
        }

        [HttpPost]
        [Route("api/productColours")]
        [Authorize(Roles = UserRoles.Admin)]
        public IList<Product> GetAllProductsWithColour(string colour)
        {
            var productsOfSpecificColour = ProductsList.Where(x => x.Colour.ToUpperInvariant() == colour.ToUpperInvariant()).ToList();
            return productsOfSpecificColour;
        }

    }
}
