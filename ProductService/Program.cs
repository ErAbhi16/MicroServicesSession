using ProductService;
using Steeltoe.Discovery.Client;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry;
using OpenTelemetry.Exporter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDiscoveryClient(builder.Configuration);
builder.Services.AddSingleton<IProductBusiness, ProductBusiness>();
var serviceName = "ProductService";

using TracerProvider? tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName, serviceVersion: "1.0.5"))
    .AddSource(serviceName)
    .AddJaegerExporter(o => o.Protocol = OpenTelemetry.Exporter.JaegerExportProtocol.HttpBinaryThrift)
    .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation()
    .Build();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//builder.Services.AddOpenTelemetryTracing(b =>
//{
//    b.AddConsoleExporter()
//    .AddSource(serviceName)
//    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName, serviceVersion: "1.0.0"))
//    .AddAspNetCoreInstrumentation()
//    .AddHttpClientInstrumentation();
//});
// Configure the HTTP request pipeline.

app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
