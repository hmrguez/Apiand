using DotNetCore.CAP;
using FastEndpoints;
using MongoDB.Driver;
using Scalar.AspNetCore;
using XXXnameXXX.Inventory.EventHandlers;
using XXXnameXXX.Inventory.Models;
using XXXnameXXX.Shared.Data;
using XXXnameXXX.Shared.Events;

var builder = WebApplication.CreateBuilder(args);

// Add data
builder.AddMongoDBClient("mongodb");
builder.AddRabbitMQClient(connectionName: "messaging");

// Add services to the container.
builder.Services.AddFastEndpoints();
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

// Add this to your service registration section
builder.Services.AddTransient<OrderCreatedEventHandler>();
builder.Services.AddScoped<IRepository<Product>, MongoRepository<Product>>();

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
