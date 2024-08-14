using InteropDoom.Utilities;

namespace AvaloniaPlayer.Doom.Events;

internal class OurEventEngine : EventEngine
{
    public OurEventEngine(ILogger logger)
    {
        MapEventHandler = new OurMapEventHandler(logger);
        EntityEventHandler = new OurEntityEventHandler(logger);
        HudEventHandler = new OurHudEventHandler(logger);
    }
}
