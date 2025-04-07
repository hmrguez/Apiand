using Apiand.Extensions.Interfaces;
using FastEndpoints;
using FastEndpoints.Swagger;
using Scalar.AspNetCore;
using XXXnameXXX.Application;
using XXXnameXXX.Infrastructure;
using XXXnameXXX.Infrastructure.DI;

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
        module.ConfigureWebAppBuilder(builder, builder.Configuration);
        module.RegisterMappings();
    }
    
    builder.AddServiceDefaults();
    builder.Services.AddOpenApi();

    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationModule).Assembly));
    
}

var app = builder.Build();
{
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseFastEndpoints();
    app.UseOpenApi();
    app.UseSwaggerGen();
    app.MapOpenApi();
    app.MapScalarApiReference(_ => _.Servers = []);

    foreach (var module in modules) module.ConfigureApplication(app);

    app.Run();
}