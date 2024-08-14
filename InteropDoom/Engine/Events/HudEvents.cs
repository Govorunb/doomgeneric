using System.Runtime.InteropServices;
using InteropDoom.Utilities;

namespace InteropDoom.Engine.Events;

/// <summary>Sent when a new message is shown in the top left.</summary>
/// <param name="text">Message text, e.g. <c>"Picked up a clip."</c></param>
public readonly struct GameMessage(string text)
{
    public readonly string Text = text;
}

internal unsafe readonly struct NativeGameMessage
{
    private readonly byte* _text;

    public static implicit operator GameMessage(NativeGameMessage unmanaged)
        => new(Marshal.PtrToStringAnsi((nint)unmanaged._text)!);
}

public abstract class HudEventHandler(ILogger? logger = null) : EventHandlerBase(logger),
    IEventHandler<GameMessage>
{
    public void Handle(GameMessage data) => OnGameMessage(data);

    protected abstract void OnGameMessage(GameMessage data);
}
