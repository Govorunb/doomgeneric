namespace InteropDoom.Engine;

public enum EventType
{
    /// <summary><inheritdoc cref="Engine.SecretDiscovered"/></summary>
    SecretDiscovered = 0,
    /// <summary><inheritdoc cref="Engine.LevelCompleted"/></summary>
    LevelCompleted = 1,
    /// <summary><inheritdoc cref="Engine.MapEntityKilled"/></summary>
    MapEntityKilled = 2,
    /// <summary><inheritdoc cref="Engine.MapEntityDamaged"/></summary>
    MapEntityDamaged = 3,
    /// <summary><inheritdoc cref="Engine.GameMessage"/></summary>
    GameMessage = 4
}

// todo: marshal/wrap all the inner structs
// god i can't wait for type unions
#region Map events

/// <summary>Player crossed a "secret" map sector.</summary>
/// <param name="Sector">The map sector's data.</param>
public record struct SecretDiscovered(nint Sector);

/// <summary>A level was just completed.</summary>
/// <param name="Episode">In DOOM II, this is always 1.</param>
/// <param name="Map">1-32.</param>
public record struct LevelCompleted(int Episode, int Map);

#endregion Map events

#region Entity events

/// <summary>An entity has died.</summary>
/// <param name="Killer">Can be <see langword="null"/> if killed by environment (floor damage, crushed by ceiling, etc).</param>
public record struct MapEntityKilled(nint Victim, nint Killer);
/// <summary>An entity was damaged.</summary>
/// <param name="Dealer">
/// Map entity that is considered the source of the damage.<br/>
/// Can be <see langword="null"/> for environment damage (toxic floor, crushed by ceiling, etc).
/// </param>
/// <param name="HealthDamage">Damage done to health.</param>
/// <param name="ArmorDamage">Damage done to armor. Applicable only when <paramref name="Victim"/> is a player.</param>
public record struct MapEntityDamaged(nint Victim, nint Dealer, int HealthDamage, int ArmorDamage);

#endregion Entity events

#region Misc events
/// <summary>Sent when a new message is shown in the top left.</summary>
/// <param name="Text">Message text, e.g. <c>"Picked up a clip."</c></param>
public record struct GameMessage(string Text);

#endregion Misc events
