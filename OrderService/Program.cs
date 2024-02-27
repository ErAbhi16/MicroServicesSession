using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OrderService.Interface;
using OrderService.Service;
using RabbitMQ.Client;
using Steeltoe.Discovery.Client;
using Steeltoe.Extensions.Configuration;
using System.Net.Sockets;
using System.Runtime.Intrinsics.Arm;
using IConnectionFactory = RabbitMQ.Client.IConnectionFactory;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddScoped<IOrderPlaceService, OrderPlaceService>();
var config = configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>();
var serviceName = "OrderService";

using TracerProvider? tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName, serviceVersion: "1.0.0"))
    .AddSource(serviceName)
    .AddJaegerExporter(o => o.Protocol = OpenTelemetry.Exporter.JaegerExportProtocol.HttpBinaryThrift)
    .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation()
    .Build();

//docker run -d --name jaeger \
//  -e COLLECTOR_ZIPKIN_HOST_PORT=:9411 \
//  -p 5775:5775 / udp \
//  -p 6831:6831 / udp \
//  -p 6832:6832 / udp \
//  -p 5778:5778 \
//  -p 16686:16686 \
//  -p 14268:14268 \
//  -p 14250:14250 \
//  -p 9411:9411 \
//  jaegertracing / all -in-one:1.22
builder.Services.AddSingleton<IConnectionFactory>(factory =>
{
    var connectionFactory = new ConnectionFactory()
    {
        HostName = config.HostName,
        Port = config.Port,
        UserName = config.UserName,
        Password = config.Password,
        VirtualHost = config.VirtualHost
    };

    // Set additional properties as needed
    // ...

    return connectionFactory;
});

//builder.Services.AddDiscoveryClient(builder.Configuration);
builder.Services.AddSingleton<IRabbitMqMessagePublisher, RabbitMqMessagePublisher>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddOpenTelemetryTracing( b => {
//    b.AddConsoleExporter()
//    .AddSource(serviceName)
//    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName, serviceVersion: "1.0.0"))
//    .AddAspNetCoreInstrumentation()
//    .AddHttpClientInstrumentation();
//    });
var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
