using Polly.Extensions.Http;
using Polly;
using Steeltoe.Discovery.Client;
using CartService;
using Steeltoe.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddScoped<ICartBusiness,CartBusiness>();
//builder.Services.AddDiscoveryClient(builder.Configuration);

builder.Services.AddHttpClient( "ProductClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7095/api/product/");
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
