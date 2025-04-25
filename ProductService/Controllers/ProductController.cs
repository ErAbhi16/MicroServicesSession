using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        readonly ILogger<ProductController> _log;
        List<Product> _products = new List<Product>();
        readonly IProductBusiness _productBusiness;

        public ProductController(ILogger<ProductController> log, IProductBusiness productBusiness)
        {
            _productBusiness = productBusiness;
            _products = ProductItems._products;
            _log = log;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            Thread.Sleep(2000);
            _log.LogInformation("showProducts has been viewed.");
            if (_products.Count() > 0)
            {
                _log.LogInformation("Product Service: products shown");
                return Ok(_products);
            }
            else
            {
                return Ok(new { message = "No products are there" });
            }
        }

        [HttpGet("{id}")]
        public ActionResult ProductById(int id)
        {
            var productById = _products.FirstOrDefault(c => c.ProductId == id);

            // Start a tracing activity for observability
            using (var activity = new ActivitySource("ProductService").StartActivity("GetProductById", ActivityKind.Server))
            {
                if (activity != null)
                {
                    activity.SetTag("product.id", id);                         // ID of the product being searched
                    activity.SetTag("product.found", productById != null);     // Whether the product was found
                    activity.SetTag("http.route", $"GET /api/product/{id}");   // Trace route
                    activity.SetTag("service.name", "ProductService");         // Service name
                }

                Console.WriteLine("Searching for product...");
                Thread.Sleep(500); // Simulated delay to mimic processing
                Console.WriteLine("Product search complete.");
            }

            if (productById == null)
            {
                throw new Exception("Service is unavailable"); // Simulate a failure case for tracing
            }

            return Ok(productById);
        }


        [HttpPost]
        public ActionResult Post([FromBody] Product product)
        {
            var count = _products.Max(c => c.ProductId);
            product.ProductId = count + 1;
            _products.Add(product);
            return Ok(_products);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var product = _products.FirstOrDefault(c => c.ProductId == id);
            if (product == null)
            {
                return BadRequest(new { message = "Incorrect product Id" });
            }
            _products.Remove(product);
            return Ok(_products);
        }


    }
}
