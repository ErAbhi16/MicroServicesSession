using OrderService.Controllers;
using RabbitMQ.Client;
using Steeltoe.Common.Order;
using System.Text;

public class RabbitMqMessagePublisher:IRabbitMqMessagePublisher
{
    private readonly IConnectionFactory _connectionFactory;

    private readonly ILogger<RabbitMqMessagePublisher> _logger;
    public RabbitMqMessagePublisher(ILogger<RabbitMqMessagePublisher> log,IConnectionFactory connectionFactory)
    {
        _logger = log;
        _connectionFactory = connectionFactory;
    }

    public void Publish(int orderID)
    {
        _logger.LogInformation($"Publishing order for {orderID}");
        using var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        var messageBytes = Encoding.UTF8.GetBytes($"[Order Placed] Order ID: {orderID}");
        channel.QueueDeclare("placeorder", true, false, false);
        channel.QueueBind("placeorder", "amq.direct", "nagp");
        channel.BasicPublish(exchange: "amq.direct", routingKey: "nagp", null, body: messageBytes);
        _logger.LogInformation($"Published order for {orderID}");
        channel.Close();
        connection.Close();

    }
}