using Apiand.Extensions.Interfaces;
using XXXnameXXX.Application;
using XXXnameXXX.Application.Todos.Queries.GetAll;
using XXXnameXXX.Infrastructure;
using XXXnameXXX.Presentation.Mutations;
using XXXnameXXX.Presentation.Queries;

var builder = WebApplication.CreateSlimBuilder(args);

List<IModule> modules =
[
    new ApplicationModule(),
    new InfraModule()
];

modules = modules 
    .Where(m => m.IsEnabled(builder.Configuration))
    .OrderBy(m => m.Order)
    .ToList();

builder.Configuration
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile("appsettings.secret.json", false, true)
    .AddEnvironmentVariables();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddAuthorization();

builder.Services.AddOpenApi();

// Add MediatR and other required services
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(GetAllTodoQuery).Assembly));

foreach (var module in modules)
{
    module.ConfigureServices(builder.Services, builder.Configuration);
    module.ConfigureWebAppBuilder(builder, builder.Configuration);
    module.RegisterMappings();
}

// Add service defaults
builder.AddServiceDefaults();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapGraphQL();

foreach (var module in modules) module.ConfigureApplication(app);

app.Run();