using Apiand.Extensions.Interfaces;
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
        services.AddInfrastructure(connectionString, databaseName);
        services.AddOpenTelemetry(configuration);
        
        // Add your services here (DO NOT REMOVE THIS LINE)
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

