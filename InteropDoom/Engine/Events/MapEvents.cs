using InteropDoom.Utilities;

namespace InteropDoom.Engine.Events;

/// <summary>Player crossed into a previously undiscovered "secret" map sector.</summary>
public readonly struct SecretDiscovered
{
    /// <summary>
    /// The map sector's data.
    /// </summary>
    public readonly nint Sector; // todo sector_t proxy
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
    IEventHandler<LevelCompleted>
{
    public void Handle(SecretDiscovered data) => OnSecretDiscovered(data);
    public void Handle(LevelCompleted data) => OnLevelCompleted(data);
    
    protected abstract void OnSecretDiscovered(SecretDiscovered data);
    protected abstract void OnLevelCompleted(LevelCompleted data);
}
