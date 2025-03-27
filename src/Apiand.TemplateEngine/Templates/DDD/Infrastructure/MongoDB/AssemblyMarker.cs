using System.Reflection;
using Apiand.Extensions.Interfaces;
using Apiand.Extensions.Service;
using XXXnameXXX.Infrastructure.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace XXXnameXXX.Infrastructure;

public class InfraModule: IModule
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDb")!;
        var databaseName = configuration["DatabaseName"]!;
        services.AddIdentity(configuration);
        services.AddServicesWithAttribute(Assembly.GetExecutingAssembly());
        services.AddInfrastructure(connectionString, databaseName);
        services.AddOpenTelemetry(configuration);
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

