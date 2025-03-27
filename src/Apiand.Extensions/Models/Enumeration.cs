using System.Reflection;

namespace Apiand.Extensions.Models;

/// <summary>
/// Represents a base class for implementing the Enumeration design pattern.
/// </summary>
/// <remarks>
/// This pattern provides an alternative to traditional enums by creating enumeration classes
/// with both integer IDs and string names. This approach allows for rich behavior on enumeration values
/// and better domain modeling than standard enumerations.
/// </remarks>
/// <param name="Id">The unique integer identifier for the enumeration value.</param>
/// <param name="Name">The descriptive name of the enumeration value.</param>
public record Enumeration(int Id, string Name)
{
    /// <summary>
    /// Gets all static instances of the specified enumeration type.
    /// </summary>
    /// <typeparam name="T">The enumeration type derived from <see cref="Enumeration"/>.</typeparam>
    /// <returns>A collection of all statically defined instances of the specified enumeration type.</returns>
    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        return fields.Select(f => f.GetValue(null))
            .Concat(properties.Select(p => p.GetValue(null)))
            .Where(value => value != null)
            .Cast<T>();
    }

    /// <summary>
    /// Finds an enumeration value by its name.
    /// </summary>
    /// <typeparam name="T">The enumeration type derived from <see cref="Enumeration"/>.</typeparam>
    /// <param name="name">The name to search for (case-insensitive).</param>
    /// <returns>The enumeration value with the specified name.</returns>
    /// <exception cref="ArgumentException">Thrown when no enumeration value with the specified name is found.</exception>
    public static T FromName<T>(string name) where T : Enumeration
    {
        return GetAll<T>().FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase))
               ?? throw new ArgumentException($"No {typeof(T).Name} with name {name} found.", nameof(name));
    }

    /// <summary>
    /// Finds an enumeration value by its ID.
    /// </summary>
    /// <typeparam name="T">The enumeration type derived from <see cref="Enumeration"/>.</typeparam>
    /// <param name="id">The ID to search for.</param>
    /// <returns>The enumeration value with the specified ID.</returns>
    /// <exception cref="ArgumentException">Thrown when no enumeration value with the specified ID is found.</exception>
    public static T FromId<T>(int id) where T : Enumeration
    {
        return GetAll<T>().FirstOrDefault(item => item.Id == id)
               ?? throw new ArgumentException($"No {typeof(T).Name} with id {id} found.", nameof(id));
    }
}