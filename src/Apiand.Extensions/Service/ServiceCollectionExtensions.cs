using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Apiand.Extensions.Service;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServicesWithAttribute(this IServiceCollection services, Assembly assembly)
    {
        var typesWithServiceAttribute = assembly.GetTypes()
            .Where(t => t.IsClass
                        && !t.IsAbstract
                        && t.GetCustomAttribute<ServiceAttribute>() != null);

        foreach (var type in typesWithServiceAttribute)
        {
            var attribute = type.GetCustomAttribute<ServiceAttribute>();
            if (attribute == null) continue;

            var serviceInterface = type.GetInterfaces().FirstOrDefault();
            if (serviceInterface == null)
            {
                Console.WriteLine($"No interface found for {type.Name}. Skipping.");
                continue;
            }

            // Register based on lifetime
            switch (attribute.Lifetime)
            {
                case ServiceLifetimeType.Scoped:
                    services.AddScoped(serviceInterface, type);
                    break;
                case ServiceLifetimeType.Transient:
                    services.AddTransient(serviceInterface, type);
                    break;
                case ServiceLifetimeType.Singleton:
                    services.AddSingleton(serviceInterface, type);
                    break;
            }

            Console.WriteLine($"Registered {type.Name} as {attribute.Lifetime} for {serviceInterface.Name}");
        }

        return services;
    }
}