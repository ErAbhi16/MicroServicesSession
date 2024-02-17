// See https://aka.ms/new-console-template for more information
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

const string hostname = "localhost";
const int port = 5672;
const string username = "guest";
const string password = "guest";

// Function to create a connection and channel
ConnectionFactory factory = new ConnectionFactory()
{
    HostName = hostname,
    Port = port,
    UserName = username,
    Password = password
};

// ... setup connection and channel
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

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
