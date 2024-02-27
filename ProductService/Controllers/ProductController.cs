using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "Deliver")]
        public ActionResult<IEnumerable<string>> Get()
        {
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
            var productbyid = _products.FirstOrDefault(c => c.ProductId == id);
            if (productbyid == null)
            {
                throw new Exception("Service is unavailable");
            }
            else
            {
                return Ok(productbyid);
            }

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
