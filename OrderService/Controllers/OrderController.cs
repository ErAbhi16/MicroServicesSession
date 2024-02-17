using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using RabbitMQ.Client;
using OrderService.Interface;

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
        public ActionResult Get([FromRoute]int cartID)
        {
            _orderPlaceService.PublishOrder(cartID);
            return Ok();


        }


    }
}
