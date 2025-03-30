using System.Reflection;
using Apiand.Extensions.Interfaces;
using Apiand.Extensions.Service;
using XXXnameXXX.Infrastructure.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XXXnameXXX.Infrastructure.Data;

namespace XXXnameXXX.Infrastructure;

public class InfraModule: IModule
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity(configuration);
        services.AddServicesWithAttribute(Assembly.GetExecutingAssembly());
        services.SetupEntityFramework(configuration);
        services.AddOpenTelemetry(configuration);
    }

    public void ConfigureApplication(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
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

