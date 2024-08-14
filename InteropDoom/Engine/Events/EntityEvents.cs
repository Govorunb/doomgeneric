using InteropDoom.Native.Structures;
using InteropDoom.Utilities;

namespace InteropDoom.Engine.Events;

/// <summary>An entity has died.</summary>
public readonly struct MapEntityKilled
{
    public readonly MapEntityProxy Victim;
    /// <summary>Map entity that dealt the final blow.</summary>
    /// <remarks>Will be uninitialized (equal to <see langword="default"/> and <see cref="MapEntityProxy.op_Implicit(MapEntityProxy)">converts</see> to <see langword="false"/>) for environment (toxic floor, crushed by ceiling, etc).</remarks>
    public readonly MapEntityProxy Killer;
}

/// <summary>An entity was damaged.</summary>
public readonly struct MapEntityDamaged
{
    public readonly MapEntityProxy Victim;
    /// <summary>Map entity that is considered the source of the damage.</summary>
    /// <remarks><inheritdoc cref="MapEntityKilled.Killer"/></remarks>
    public readonly MapEntityProxy Dealer;
    /// <summary>Damage done directly to health.</summary>
    public readonly int HealthDamage;
    /// <summary>Damage absorbed by armor. Applicable only when <see cref="Victim"/> is a player (check <see cref="MapEntityProxy.IsPlayer"/>).</summary>
    public readonly int ArmorDamage;
}

public abstract class EntityEventHandler(ILogger? logger = null) : EventHandlerBase(logger),
      IEventHandler<MapEntityDamaged>,
      IEventHandler<MapEntityKilled>
{
    public void Handle(MapEntityDamaged data) => OnMapEntityDamaged(data);
    public void Handle(MapEntityKilled data) => OnMapEntityKilled(data);
    
    protected abstract void OnMapEntityDamaged(MapEntityDamaged data);
    protected abstract void OnMapEntityKilled(MapEntityKilled data);
}
