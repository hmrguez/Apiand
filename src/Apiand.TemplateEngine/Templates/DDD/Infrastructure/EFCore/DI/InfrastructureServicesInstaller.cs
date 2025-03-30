using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XXXnameXXX.Application.Contracts;
using XXXnameXXX.Infrastructure.Contracts;
using XXXnameXXX.Infrastructure.Data;

namespace XXXnameXXX.Infrastructure.DI;

public static class InfrastructureServicesInstaller
{
    public static void SetupEntityFramework(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var dbProvider = configuration["Database:Provider"]?.ToLower() ?? "sqlserver";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (dbProvider == "postgres")
            {
                options.UseNpgsql(connectionString);
            }
            else
            {
                options.UseSqlServer(connectionString);
            }
        });
        
        // Register repository implementation
        services.AddScoped(typeof(IRepository<>), typeof(EfCoreRepository<>));
    }
}