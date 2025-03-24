using XXXnameXXX.Api;
using Apiand.Extensions.Interfaces;
using XXXnameXXX.Application;
using XXXnameXXX.Application.DI;
using FastEndpoints;
using FastEndpoints.Swagger;
using XXXnameXXX.Infrastructure;
using XXXnameXXX.Infrastructure.DI;


var builder = WebApplication.CreateSlimBuilder(args);

var modules = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(a => a.GetTypes())
    .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
    .Select(t => (IModule)Activator.CreateInstance(t)!)
    .Where(m => m.IsEnabled(builder.Configuration))
    .OrderBy(m => m.Order)
    .ToList();

{
    builder.Configuration
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile("appsettings.secret.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    builder.Services
        .AddFastEndpoints()
        .SwaggerDocument();

    foreach (var module in modules)
    {
        module.ConfigureServices(builder.Services, builder.Configuration);
        module.RegisterMappings();
    }

    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    });

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

    foreach (var module in modules)
    {
        module.ConfigureApplication(app);
    }

    app.Run();
}