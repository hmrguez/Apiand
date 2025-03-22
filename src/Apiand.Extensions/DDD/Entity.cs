namespace Apiand.Extensions.DDD;

/// <summary>
/// Base class for all domain entities in DDD architecture.
/// An entity is an object with a unique identity that persists throughout its lifetime,
/// even when its attributes change.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Unique identifier for this entity. Only set when inserting into the DB
    /// </summary>
    public string Id { get; set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    public Entity()
    {
        Id = Guid.NewGuid().ToString();
        CreatedAt = DateTime.UtcNow;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode() => Id.GetHashCode();
    
    // == and != operators
    public static bool operator ==(Entity? a, Entity? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity? a, Entity? b)
    {
        return !(a == b);
    }
}