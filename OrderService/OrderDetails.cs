namespace OrderService
{
    public class OrderDetails
    {

        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string PaymentStatus { get; set; }

        public List<int> ProductIds { get; set; }
        public int OrderAmount { get; set; }
        public string DeliverStatus { get; set; }
        public string DeliveryStatusReason { get; set; }

    }

    public static class CustomerDetails
    {

        public static int CustomerID = 2;
    }
}
