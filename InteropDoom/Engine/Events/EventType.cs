namespace InteropDoom.Engine.Events;

// yearning for type unions...
public enum EventType
{
    /// <summary><inheritdoc cref="Events.SecretDiscovered"/></summary>
    SecretDiscovered,
    /// <summary><inheritdoc cref="Events.ItemPickedUp"/></summary>
    ItemPickedUp,
    /// <summary><inheritdoc cref="Events.LevelCompleted"/></summary>
    LevelCompleted,
    /// <summary><inheritdoc cref="Events.MapEntityKilled"/></summary>
    MapEntityKilled,
    /// <summary><inheritdoc cref="Events.MapEntityDamaged"/></summary>
    MapEntityDamaged,
    /// <summary><inheritdoc cref="Events.GameMessage"/></summary>
    GameMessage
}

// todo: marshal/wrap all of the structs inside the event data
// see MapEntityProxy for an example
