namespace InteropDoom.Engine.Events;

// yearning for type unions...
public enum EventType
{
    /// <summary><inheritdoc cref="Events.SecretDiscovered"/></summary>
    SecretDiscovered = 0,
    /// <summary><inheritdoc cref="Events.LevelCompleted"/></summary>
    LevelCompleted = 1,
    /// <summary><inheritdoc cref="Events.MapEntityKilled"/></summary>
    MapEntityKilled = 2,
    /// <summary><inheritdoc cref="Events.MapEntityDamaged"/></summary>
    MapEntityDamaged = 3,
    /// <summary><inheritdoc cref="Events.GameMessage"/></summary>
    GameMessage = 4
}

// todo: marshal/wrap all of the structs inside the event data
// see MapEntityProxy for an example
