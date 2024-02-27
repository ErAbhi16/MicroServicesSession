using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using RabbitMQ.Client;
using OrderService.Interface;
using System.Net.Http;

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
        public async Task<ActionResult> GetAsync([FromRoute]int cartID)
        {
            _log.LogInformation($"OrderService cart id: {cartID} ");
            using var httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7095/api/product/3");

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


    }
}
