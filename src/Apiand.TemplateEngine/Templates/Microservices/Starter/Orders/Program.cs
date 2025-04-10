using DotNetCore.CAP;
using FastEndpoints;
using MongoDB.Driver;
using Scalar.AspNetCore;
using XXXnameXXX.Shared.Data;
using XXXnameXXX.Shared.Events;
using Order = XXXnameXXX.Orders.Models.Order;

var builder = WebApplication.CreateBuilder(args);

// Add data
builder.AddMongoDBClient("mongodb");
builder.AddRabbitMQClient(connectionName: "messaging");

// Add services to the container.
builder.Services.AddFastEndpoints().AddSwaggerDocument();
builder.Services.AddOpenApi();
builder.AddServiceDefaults();

builder.Services.AddCap(x =>
{
    x.UseRabbitMQ(opt =>
    {
        opt.ConnectionFactoryOptions = factory =>
        {
            factory.Uri = new Uri(builder.Configuration.GetConnectionString("messaging")!);
        };
    });
    x.UseInMemoryStorage();
});

builder.Services.AddScoped<IRepository<Order>, MongoRepository<Order>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(x => x.Servers = []);
}

app.UseFastEndpoints();
app.UseHttpsRedirection();

app.Run();