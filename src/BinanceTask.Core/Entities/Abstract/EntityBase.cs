namespace BinanceTask.Core.Entities.Abstract;

/// <summary>
/// This class represents the base entity for all entities.
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// Gets or sets the entity identifier.
    /// </summary>
    public int Id { get; protected set; }
}

