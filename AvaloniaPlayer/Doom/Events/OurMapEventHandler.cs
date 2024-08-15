using InteropDoom.Engine.Events;
using InteropDoom.Utilities;

namespace AvaloniaPlayer.Doom.Events;
internal class OurMapEventHandler(ILogger logger) : MapEventHandler(logger)
{
    protected override void OnSecretDiscovered(SecretDiscovered data)
    {
        Logger?.LogInfo($"Discovered a secret! ({data.Count}/{data.Total})");
        if (data.Count == data.Total)
            Logger?.LogInfo($"All secrets discovered! Well done!");
    }
    protected override void OnItemPickedUp(ItemPickedUp data)
    {
        // Logger?.LogInfo($"Picked up an item! ({data.Count}/{data.Total})");
        if (data.Count == data.Total)
            Logger?.LogInfo($"All items picked up! Well done!");
    }
    protected override void OnLevelCompleted(LevelCompleted data)
    {
        Logger?.LogInfo($"Completed E{data.Episode}M{data.Map}");
    }
}
