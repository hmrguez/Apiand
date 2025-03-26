using Apiand.Extensions.Interfaces;
using FastEndpoints;
using FastEndpoints.Swagger;
using XXXnameXXX.Application;
using XXXnameXXX.Infrastructure;
using XXXnameXXX.Infrastructure.DI;

var builder = WebApplication.CreateSlimBuilder(args);

var assemblies = new[]
{
    typeof(ApplicationModule).Assembly,
    typeof(InfraModule).Assembly
};

var modules = assemblies
    .SelectMany(a => a.GetTypes())
    .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
    .Select(t => (IModule)Activator.CreateInstance(t)!)
    .Where(m => m.IsEnabled(builder.Configuration))
    .OrderBy(m => m.Order)
    .ToList();

{
    builder.Configuration
        .AddJsonFile("appsettings.json", false, true)
        .AddJsonFile("appsettings.secret.json", false, true)
        .AddEnvironmentVariables();

    builder.Services
        .AddFastEndpoints()
        .SwaggerDocument();

    foreach (var module in modules)
    {
        module.ConfigureServices(builder.Services, builder.Configuration);
        module.RegisterMappings();
    }

    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationModule).Assembly));

    var config = builder.Configuration;

    builder.Logging.AddOpenTelemetry(config);
}

var app = builder.Build();
{
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseFastEndpoints();
    app.UseOpenApi();
    app.UseSwaggerGen();

    foreach (var module in modules) module.ConfigureApplication(app);

    app.Run();
}