using InteropDoom.Utilities;

namespace InteropDoom.Engine.Events;

/// <summary>Player crossed into a previously undiscovered "secret" map sector.</summary>
public readonly struct SecretDiscovered
{
    /// <summary>The number of secrets discovered so far.</summary>
    public readonly int Count;
    /// <summary>The total number of secrets on the map.</summary>
    public readonly int Total;
}

public readonly struct ItemPickedUp
{
    /// <summary>The number of items picked up so far.</summary>
    public readonly int Count;
    /// <summary>The total number of items on the map.</summary>
    public readonly int Total;
}

/// <summary>A level was just completed.</summary>
public readonly struct LevelCompleted
{
    /// <summary>In DOOM II, this is always 1.</summary>
    public readonly int Episode;
    /// <summary>1-32.</summary>
    public readonly int Map;
}

public abstract class MapEventHandler(ILogger? logger = null) : EventHandlerBase(logger),
    IEventHandler<SecretDiscovered>,
    IEventHandler<ItemPickedUp>,
    IEventHandler<LevelCompleted>
{
    public void Handle(SecretDiscovered data) => OnSecretDiscovered(data);
    public void Handle(ItemPickedUp data) => OnItemPickedUp(data);
    public void Handle(LevelCompleted data) => OnLevelCompleted(data);
    
    protected abstract void OnSecretDiscovered(SecretDiscovered data);
    protected abstract void OnItemPickedUp(ItemPickedUp data);
    protected abstract void OnLevelCompleted(LevelCompleted data);
}
