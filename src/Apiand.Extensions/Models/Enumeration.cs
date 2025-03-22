using System.Reflection;

namespace Apiand.Extensions.Models;

public record Enumeration(int Id, string Name)
{
    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        return fields.Select(f => f.GetValue(null))
            .Concat(properties.Select(p => p.GetValue(null)))
            .Where(value => value != null)
            .Cast<T>();
    }

    public static T FromName<T>(string name) where T : Enumeration
    {
        return GetAll<T>().FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase))
               ?? throw new ArgumentException($"No {typeof(T).Name} with name {name} found.", nameof(name));
    }

    public static T FromId<T>(int id) where T : Enumeration
    {
        return GetAll<T>().FirstOrDefault(item => item.Id == id)
               ?? throw new ArgumentException($"No {typeof(T).Name} with id {id} found.", nameof(id));
    }
}