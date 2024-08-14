using InteropDoom.Engine.Events;
using InteropDoom.Utilities;

namespace AvaloniaPlayer.Doom.Events;
internal class OurMapEventHandler(ILogger logger) : MapEventHandler(logger)
{
    protected override void OnLevelCompleted(LevelCompleted data)
    {
        Logger?.LogInfo($"Completed E{data.Episode}M{data.Map}");
    }
    protected override void OnSecretDiscovered(SecretDiscovered data)
    {
        Logger?.LogInfo($"Discovered a secret! {data.Sector}");
    }
}
