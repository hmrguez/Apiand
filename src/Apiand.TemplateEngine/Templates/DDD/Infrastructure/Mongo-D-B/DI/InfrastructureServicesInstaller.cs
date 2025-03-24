using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using XXXnameXXX.Application.Contracts;
using XXXnameXXX.Application.Identity.Services;
using XXXnameXXX.Infrastructure.Contracts;
using XXXnameXXX.Infrastructure.Identity;

namespace XXXnameXXX.Infrastructure.DI;

public static class InfrastructureServicesInstaller
{
    public static void AddInfrastructure(this IServiceCollection services, string mongoConnectionString, string databaseName)
    {
        var client = new MongoClient(mongoConnectionString);
        var database = client.GetDatabase(databaseName);
        
        services.AddSingleton(database);
        services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));
        
        // Add other services here
        services.AddScoped<IUserService, UserService>();
    }
}