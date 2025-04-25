using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using RabbitMQ.Client;
using OrderService.Interface;
using System.Net.Http;
using OrderService.Model;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private OrderDetails _orderDetails = new OrderDetails();
        readonly ILogger<OrderController> _log;
        private readonly IOrderPlaceService _orderPlaceService;
        public OrderController(ILogger<OrderController> log, IOrderPlaceService orderPlaceService)
        {
            _log = log;
            _orderPlaceService = orderPlaceService;
        }


        [HttpGet]
        public ActionResult Get()
        {
            var count = OrderDatabase.orderdb.Max(c => c.OrderId);

            IEnumerable<OrderDetails> CustomerOrders = OrderDatabase.orderdb.Where(c => c.CustomerId == CustomerDetails.CustomerID);
            if (CustomerOrders.Count() > 0)
            {
                _log.LogInformation("OrderService : Orders successfully shown ");
                return Ok(CustomerOrders.Select(c => new { c.OrderId, c.CustomerId, c.PaymentStatus, c.DeliverStatus }));
            }
            else
            {
                _log.LogInformation("OrderService : There are no orders");
                return Ok(new { message = "There are no orders...!!!" });
            }

        }

        [HttpGet("{cartID}")]
        public async Task<ActionResult> GetAsync([FromRoute] int cartID)
        {
            _log.LogInformation($"OrderService received cart id: {cartID}");

            // Simulate a dummy cart lookup (you would normally fetch from DB or service)
            CartModel dummyCart = GetDummyCartById(cartID);
            if (dummyCart == null)
            {
                return NotFound($"Cart with ID {cartID} not found.");
            }

            int productId = dummyCart.ProductId;

            using var httpClient = new HttpClient();
            string productUrl = $"https://localhost:7095/api/product/{productId}";
            HttpResponseMessage response = await httpClient.GetAsync(productUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response body: {responseBody}");
            }
            else
            {
                Console.WriteLine($"HTTP status code: {response.StatusCode}");
            }

            return Ok();
        }

        // Dummy method for simulating cart data
        private CartModel GetDummyCartById(int cartId)
        {
            // In a real scenario, you'd look up the DB or a cache
            var dummyCarts = new List<CartModel>
    {
        new CartModel { CartId = 1, ProductId = 1 },
        new CartModel { CartId = 2, ProductId = 2 },
        new CartModel { CartId = 3, ProductId = 3 }
    };

            return dummyCarts.FirstOrDefault(c => c.CartId == cartId);
        }



    }
}
