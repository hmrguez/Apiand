using XXXnameXXX.XXXserviceXXX.Mutations;
using XXXnameXXX.XXXserviceXXX.Queries;
using XXXnameXXX.XXXserviceXXX.Services;

var builder = WebApplication.CreateBuilder(args);

// Add data
builder.AddMongoDBClient("mongodb");
builder.AddRabbitMQClient(connectionName: "messaging");

// Add services for GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

builder.Services.AddSingleton<TodoRepository>();

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
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

// Map the GraphQL endpoint
app.MapGraphQL();

app.Run();