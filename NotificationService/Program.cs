// See https://aka.ms/new-console-template for more information
using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
// Function to create a connection and channel
ConnectionFactory factory = new ConnectionFactory()
{
    HostName = configuration["RabbitMQ:HostName"],
    Port = Convert.ToInt32(configuration["RabbitMQ:Port"]),
    UserName = configuration["RabbitMQ:UserName"],
    Password = configuration["RabbitMQ:Password"],
    VirtualHost = configuration["RabbitMQ:VirtualHost"]
};
Console.WriteLine($"{factory.HostName}");
// ... setup connection and channel
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
channel.QueueDeclare("placeorder", true, false, false);
channel.QueueBind("placeorder", "amq.direct", "nagp");
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
    Console.WriteLine($"Received message: {message}");
    // Process the message based on its content
};

channel.BasicConsume(queue: "placeorder", autoAck: true, consumer: consumer);

Console.WriteLine("Listening for notifications...");
Console.ReadLine();
