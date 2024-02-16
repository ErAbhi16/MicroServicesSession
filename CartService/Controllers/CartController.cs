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

        public CartController(IConfiguration config, ILogger<CartController> log)
        {
            _config = config;
            _log = log;
        }

        // GET api/values/5
        //This shows details of cart for the logged in user.
        [HttpGet("showcart")]
        public ActionResult<string> Get()
        {
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


    }
}
