using Microsoft.AspNetCore.Connections;
using OrderService.Interface;
using OrderService.Service;
using RabbitMQ.Client;
using Steeltoe.Discovery.Client;
using Steeltoe.Extensions.Configuration;
using IConnectionFactory = RabbitMQ.Client.IConnectionFactory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDiscoveryClient(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddScoped<IOrderPlaceService, OrderPlaceService>();
builder.Services.AddSingleton<IConnectionFactory>(factory =>
{
    var connectionFactory = new ConnectionFactory()
    {
        HostName = "localhost",
        Port = 5672,
        UserName = "guest",
        Password = "guest",
        VirtualHost = "/"
    };

    // Set additional properties as needed
    // ...

    return connectionFactory;
});
builder.Services.AddSingleton<IRabbitMqMessagePublisher, RabbitMqMessagePublisher>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
