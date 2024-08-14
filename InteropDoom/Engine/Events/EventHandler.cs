using InteropDoom.Utilities;

namespace InteropDoom.Engine.Events;

public interface IEventHandler<T>
    where T : struct
{
    void Handle(T data);
}

public abstract class EventHandlerBase(ILogger? logger)
{
    public ILogger? Logger { get; protected internal set; } = logger;
}