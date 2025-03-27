using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Apiand.Extensions.Service;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> that provide automatic service registration
/// capabilities based on attributes.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Automatically registers all services in the specified assembly that are decorated with the
    /// <see cref="ServiceAttribute"/>.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="assembly">The assembly to scan for services.</param>
    /// <returns>The original service collection for chaining.</returns>
    /// <remarks>
    /// This method scans the provided assembly for classes that:
    /// <list type="bullet">
    ///   <item>Are not abstract</item>
    ///   <item>Are decorated with the <see cref="ServiceAttribute"/></item>
    ///   <item>Implement at least one interface (which will be used as the service type)</item>
    /// </list>
    /// Services are registered with the dependency injection container according to the lifetime
    /// specified in the <see cref="ServiceAttribute"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// // In Startup.cs or Program.cs
    /// services.AddServicesWithAttribute(typeof(Program).Assembly);
    /// </code>
    /// </example>
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