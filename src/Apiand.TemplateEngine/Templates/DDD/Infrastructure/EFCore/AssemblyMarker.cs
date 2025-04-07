using System.Reflection;
using Apiand.Extensions.Interfaces;
using Apiand.Extensions.Service;
using XXXnameXXX.Infrastructure.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XXXnameXXX.Application.Contracts;
using XXXnameXXX.Infrastructure.Contracts;
using XXXnameXXX.Infrastructure.Data;

namespace XXXnameXXX.Infrastructure;

public class InfraModule : IModule
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
    }


    public void ConfigureWebAppBuilder(IHostApplicationBuilder builder, IConfiguration configuration)
    {
        var dbProvider = configuration["Database:Provider"]?.ToLower() ?? "sqlserver";
        
        if (builder.Environment.IsDevelopment() && dbProvider == "postgres")
        {
            builder.AddNpgsqlDbContext<ApplicationDbContext>(connectionName: "postgresdb");
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
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
        }

        builder.Services.AddScoped(typeof(IRepository<>), typeof(EfCoreRepository<>));

        builder.Services.AddIdentity(configuration);
        builder.Services.AddServicesWithAttribute(Assembly.GetExecutingAssembly());
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