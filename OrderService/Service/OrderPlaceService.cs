using OrderService.Interface;

namespace OrderService.Service
{
    public class OrderPlaceService : IOrderPlaceService
    {
        private readonly IRabbitMqMessagePublisher _rabbitMqMessagePublisher;
        public OrderPlaceService(IRabbitMqMessagePublisher rabbitMqMessagePublisher)
        {
            _rabbitMqMessagePublisher = rabbitMqMessagePublisher;
        }
        public void PublishOrder(int cartID)
        {
            //business logic to create order
            _rabbitMqMessagePublisher.Publish(cartID*2);
        }
    }
}
