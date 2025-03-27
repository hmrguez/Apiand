namespace Apiand.Extensions.Service;

/// <summary>
/// Defines the lifetime of a service registered with the dependency injection container.
/// </summary>
public enum ServiceLifetimeType
{
    /// <summary>
    /// A new instance is created for each scope (e.g., per request in a web application).
    /// </summary>
    Scoped,
    
    /// <summary>
    /// A new instance is created each time the service is requested.
    /// </summary>
    Transient,
    
    /// <summary>
    /// A single instance is created and shared throughout the application's lifetime.
    /// </summary>
    Singleton
}

/// <summary>
/// Attribute used to mark classes for automatic dependency injection registration.
/// </summary>
/// <remarks>
/// When applied to a class, this attribute indicates that the class should be registered
/// with the dependency injection container using the specified lifetime.
/// Use with <c>services.AddServicesWithAttribute()</c> extension method.
/// </remarks>
/// <example>
/// <code>
/// [Service(ServiceLifetimeType.Scoped)]
/// public class UserService : IUserService
/// {
///     // Implementation
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ServiceAttribute(ServiceLifetimeType lifetime = ServiceLifetimeType.Scoped) : Attribute
{
    /// <summary>
    /// Gets the lifetime of the service in the dependency injection container.
    /// </summary>
    /// <value>
    /// Default is <see cref="ServiceLifetimeType.Scoped"/>.
    /// </value>
    public ServiceLifetimeType Lifetime { get; } = lifetime;
}