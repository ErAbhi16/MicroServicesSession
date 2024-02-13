using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers
{
    [ApiController]
    public class ValuesController : Controller
    {
        private OrderDetails _orderDetails = new OrderDetails();
        readonly ILogger<ValuesController> _log;
        //private static int _count = 0;

        public ValuesController(ILogger<ValuesController> log)
        {
            _log = log;
        }
        [HttpGet("showcart")]
        public string GetD()
        {
            return "Cart from Order";
        }

        [HttpGet("showorders")]
        public ActionResult Get()
        {
            //_count++;
            //if (_count <= 3)
            //{
            //    Thread.Sleep(3000);
            //}

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

        [HttpGet("makepayment/{id}")]
        public ActionResult<string> Get(int id)
        {
            OrderDetails order = OrderDatabase.orderdb.FirstOrDefault(c => c.CustomerId == CustomerDetails.CustomerID && c.OrderId == id && c.PaymentStatus == "Pending");

            if (order == null)
            {
                return BadRequest(new { message = "Wrong Order ID" });
            }
            else
            {
                order.PaymentStatus = "PaymentDone";
                return Ok(order);
            }
        }

    }
}
