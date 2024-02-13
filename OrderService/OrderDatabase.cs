namespace OrderService
{
    public static class OrderDatabase
    {
        public static List<OrderDetails> orderdb = new List<OrderDetails> {
            new OrderDetails{OrderId=1,OrderAmount=3300,PaymentStatus = "Pending",CustomerId = 2,ProductIds =new List<int>{ 1,2,3} ,DeliverStatus="Undelivered",DeliveryStatusReason ="Packing in Progress" },
            new OrderDetails{OrderId=2,OrderAmount=3400,PaymentStatus = "Pending",CustomerId = 2,ProductIds=new List<int>{ 1,2,3},DeliverStatus="Undelivered",DeliveryStatusReason ="Packing in Progress" },
            new OrderDetails{OrderId=3,OrderAmount=3500,PaymentStatus = "Pending",CustomerId = 2,ProductIds=new List<int>{ 1,2,3},DeliverStatus="Undelivered",DeliveryStatusReason ="Packing in Progress" },
        };
    }
}
