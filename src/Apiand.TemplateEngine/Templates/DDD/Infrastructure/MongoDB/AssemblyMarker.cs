using System.Reflection;
using Apiand.Extensions.Interfaces;
using Apiand.Extensions.Service;
using XXXnameXXX.Infrastructure.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using XXXnameXXX.Application.Contracts;
using XXXnameXXX.Infrastructure.Contracts;

namespace XXXnameXXX.Infrastructure;

public class InfraModule : IModule
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void ConfigureWebAppBuilder(IHostApplicationBuilder builder, IConfiguration configuration)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.AddMongoDBClient(connectionName: "mongodb");
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")!;
            var databaseName = configuration["Database:Name"]!;
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            builder.Services.AddSingleton(database);
        }
        
        // services.AddOpenTelemetry(configuration);
        
        builder.Services.AddIdentity(configuration);
        builder.Services.AddServicesWithAttribute(Assembly.GetExecutingAssembly());
        builder.Services.AddInfrastructure();

        builder.Services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));
    }

    public void ConfigureApplication(IApplicationBuilder app)
    {
    }

    public void RegisterMappings()
    {
    }

    public string Name { get; } = "Infrastructure";
    public int Order { get; } = 1;

    public bool IsEnabled(IConfiguration configuration)
    {
        return true;
    }
}