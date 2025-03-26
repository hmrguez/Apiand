namespace Apiand.Extensions.Service;

public enum ServiceLifetimeType
{
    Scoped,
    Transient,
    Singleton
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ServiceAttribute(ServiceLifetimeType lifetime = ServiceLifetimeType.Scoped) : Attribute
{
    public ServiceLifetimeType Lifetime { get; } = lifetime;
}