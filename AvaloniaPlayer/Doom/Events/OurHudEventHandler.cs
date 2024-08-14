using InteropDoom.Engine.Events;
using InteropDoom.Utilities;

namespace AvaloniaPlayer.Doom.Events;
internal class OurHudEventHandler(ILogger logger) : HudEventHandler(logger)
{
    protected override void OnGameMessage(GameMessage data)
    {
        Logger?.LogInfo($"Game message: {data.Text}");
    }
}
