using FastEndpoints;
using Scalar.AspNetCore;

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