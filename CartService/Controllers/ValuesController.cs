using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers
{
    [ApiController]
    public class ValuesController : Controller
    {
        private CartDetails cart = new CartDetails();
        private readonly IConfiguration _config;
        readonly ILogger<ValuesController> _log;

        public ValuesController(IConfiguration config, ILogger<ValuesController> log)
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


        // DELETE api/values/5
        //This is used to delete any product from the cart
        [HttpDelete("deletefromcart/{id}")]
        public ActionResult Delete(int id)
        {
            List<CartDetails> deleteproduct = CartDatabase.cartdb.Where(c => c.CustomerID == CustomerDetails.CustomerID && c.ProductsID == id).ToList();
            if (deleteproduct.Count() <= 0)
            {
                _log.LogInformation("From cart service: tried to delete wrong product from cart");
                return BadRequest(new { message = "This Product doesnot exist in ur cart" });
            }

            CartDatabase.cartdb.Remove(deleteproduct.FirstOrDefault());
            IEnumerable<CartDetails> products = CartDatabase.cartdb.Where(c => c.CustomerID == CustomerDetails.CustomerID);
            if (products.Count() > 0)
            {
                return Ok(products);
            }
            _log.LogInformation("From cart service: product deleted");
            return Ok(new { message = "Your cart is empty now...!!" });

        }

    }
}
