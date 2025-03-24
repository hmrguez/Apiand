using System.Reflection;
using Apiand.TemplateEngine.Utils;

namespace Apiand.TemplateEngine.Models;

/// <summary>
/// Factory class for discovering and creating instances of architecture types.
/// </summary>
public static class ArchitectureTypeFactory
{
    /// <summary>
    /// Lazily loads all available architecture types when first accessed.
    /// </summary>
    private static readonly Lazy<Dictionary<string, Type>> ArchitectureTypes = new(LoadArchitectureTypes);

    /// <summary>
    /// Gets a dictionary of all available architecture types, keyed by their lowercase names.
    /// </summary>
    public static IReadOnlyDictionary<string, Type> AvailableArchitectureTypes => ArchitectureTypes.Value;

    /// <summary>
    /// Creates a new instance of an architecture type by its name.
    /// </summary>
    /// <param name="name">The name of the architecture type to create (case-insensitive)</param>
    /// <returns>A new instance of the specified architecture type</returns>
    /// <exception cref="ArgumentException">Thrown when the name is null or empty</exception>
    /// <exception cref="InvalidOperationException">Thrown when no architecture type matches the name</exception>
    public static ArchitectureType Create(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Architecture type name cannot be null or empty", nameof(name));

        if (!ArchitectureTypes.Value.TryGetValue(name.ToLowerInvariant(), out var type))
            throw new InvalidOperationException($"Architecture type '{name}' not found");

        return (ArchitectureType)Activator.CreateInstance(type)!;
    }

    /// <summary>
    /// Loads all architecture types from the assembly by using reflection.
    /// </summary>
    /// <returns>A dictionary mapping lowercase architecture names to their corresponding types</returns>
    private static Dictionary<string, Type> LoadArchitectureTypes()
    {
        var architectureTypeClass = typeof(ArchitectureType);

        return Assembly.GetAssembly(architectureTypeClass)!
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && architectureTypeClass.IsAssignableFrom(t))
            .Select(t => (Type: t, Instance: (ArchitectureType)Activator.CreateInstance(t)!))
            .ToDictionary(
                x => x.Instance.Name.ToLowerInvariant(),
                x => x.Type
            );
    }
    
    /// <summary>
    /// Gets the appropriate TemplateConfiguration type for a given architecture name.
    /// </summary>
    /// <param name="architectureName">The name of the architecture (case-insensitive)</param>
    /// <returns>The Type of the TemplateConfiguration for the specified architecture</returns>
    /// <exception cref="ArgumentException">Thrown when architectureName is null or empty</exception>
    /// <exception cref="InvalidOperationException">Thrown when no configuration type is found for the architecture</exception>
    public static Type GetConfigurationType(string architectureName)
    {
        if (string.IsNullOrEmpty(architectureName))
            throw new ArgumentException("Architecture name cannot be null or empty", nameof(architectureName));
    
        var configType = Assembly.GetAssembly(typeof(TemplateConfiguration))!
            .GetTypes()
            .FirstOrDefault(t => 
                typeof(TemplateConfiguration).IsAssignableFrom(t) &&
                t.GetCustomAttribute<ArchitectureAttribute>()?.ArchitectureName
                    .Equals(architectureName, StringComparison.OrdinalIgnoreCase) == true);
                
        return configType ?? typeof(TemplateConfiguration);
    }
    
    
    public static T? GetCommandImplementations<T>(string architectureName) where T : ICommandSpecification
    {
        var t = typeof(T);
        
        // Get all types from the executing assembly that implement IAddService
        var implementations = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes()
                .Where(type => t.IsAssignableFrom(type) && 
                              !type.IsInterface && 
                              !type.IsAbstract))
            .Select(type => (T)Activator.CreateInstance(type)!)
            .Where(instance => instance != null && 
                   string.Equals(instance.ArchName, architectureName, StringComparison.OrdinalIgnoreCase));
        
        return implementations.FirstOrDefault();
    }
}