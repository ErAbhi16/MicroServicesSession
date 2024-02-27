using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private CartDetails cart = new CartDetails();
        private readonly IConfiguration _config;
        readonly ILogger<CartController> _log;
        readonly ICartBusiness _cartBusiness;

        public CartController(IConfiguration config, ILogger<CartController> log, ICartBusiness cartBusiness)
        {
            _config = config;
            _log = log;
            _cartBusiness = cartBusiness;
        }

        // GET api/values/5
        //This shows details of cart for the logged in user.
        [HttpGet("showcart")]
        public ActionResult<string> Get()
        {
            Thread.Sleep(2000);
            IEnumerable<CartDetails> productsInCart = CartDatabase.cartdb.Where(c => c.CustomerID == CustomerDetails.CustomerID);
            if (productsInCart.Count() > 0)
            {
                _log.LogInformation("From cart service: Products shown from cart");
                return Ok(new { message = "Showing Products from customer cart", products = productsInCart });
            }
            else
            {
                _log.LogInformation("from cart service: Cart is empty");
                return Ok(new { message = "Your cart is empty.!! Go shopping...!!!" });
            };
        }

        [HttpPut]
        public async Task<ActionResult<string>> PutAsync([FromBody] Product product)
        {
            var response = await _cartBusiness.GetProductAsync(product.ProductId);
            if (response)
            {
                _log.LogInformation("From cart service: Update Cart successfully");
                return Ok(new { message = "Update Cart successfully" });
            }
            else
            {
                _log.LogInformation("from cart service: Update failed as Product Doesn't exists");
                return Ok(new { message = "Update failed as Product Doesn't exists !!!" });
            };
        }


    }
}
