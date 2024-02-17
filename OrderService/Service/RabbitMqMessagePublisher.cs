using RabbitMQ.Client;
using Steeltoe.Common.Order;
using System.Text;

public class RabbitMqMessagePublisher:IRabbitMqMessagePublisher
{
    private readonly IConnectionFactory _connectionFactory;
    public RabbitMqMessagePublisher(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void Publish(int orderID)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        var messageBytes = Encoding.UTF8.GetBytes($"[Order Placed] Order ID: {orderID}");
        channel.QueueDeclare("placeorder", true, false, false);
        channel.QueueBind("placeorder", "amq.direct", "nagp");
        channel.BasicPublish(exchange: "amq.direct", routingKey: "nagp", null, body: messageBytes);
        channel.Close();
        connection.Close();

    }
}