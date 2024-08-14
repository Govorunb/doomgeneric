using InteropDoom.Engine.Events;
using InteropDoom.Utilities;

namespace InteropDoom.Engine;

public abstract class EventEngine
{
    public MapEventHandler? MapEventHandler { get; protected set; }
    public EntityEventHandler? EntityEventHandler { get; protected set; }
    public HudEventHandler? HudEventHandler { get; protected set; }
    public ILogger? Logger { get; protected internal set; }
    private readonly Dictionary<EventType, Action<nint>> _handlers;

    protected EventEngine()
    {
        // this used to be a switch statement and i can't tell what's worse
        // at least it's slightly more compact in loc
        _handlers = new()
        {
            // map events
            [EventType.SecretDiscovered] = (dataPtr) => MapEventHandler?.Handle(dataPtr.ToStruct<SecretDiscovered>()),
            [EventType.LevelCompleted] = (dataPtr) => MapEventHandler?.Handle(dataPtr.ToStruct<LevelCompleted>()),
            // entity events
            [EventType.MapEntityDamaged] = (dataPtr) => EntityEventHandler?.Handle(dataPtr.ToStruct<MapEntityDamaged>()),
            [EventType.MapEntityKilled] = (dataPtr) => EntityEventHandler?.Handle(dataPtr.ToStruct<MapEntityKilled>()),
            // hud events
            [EventType.GameMessage] = (dataPtr) => HudEventHandler?.Handle(dataPtr.ToStruct<NativeGameMessage>())
        };
    }

    internal void DispatchEvent(EventType type, nint dataPtr)
    {
        if (_handlers.TryGetValue(type, out var handler))
            handler(dataPtr);
        else
            Logger?.LogDebug($"Unknown event {type}");
    }
}
