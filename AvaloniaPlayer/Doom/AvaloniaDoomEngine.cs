using AvaloniaPlayer.Doom.Audio;
using InteropDoom;

namespace AvaloniaPlayer.Doom;
internal sealed class AvaloniaDoomEngine : DoomEngine
{
    public event Action? DrawFrame;
    public event Action<ScreenBuffer>? Init;
    public event Action<int>? Exit;
    public event Action<string>? WindowTitleChanged;

    public AvaloniaDoomEngine() : base()
    {
        SoundEngine = new SfxOutput();
        MusicEngine = new MusicOutput();
        Dispatcher = new AvaloniaMainThreadDispatcher();
        AudioOutput.Init();
    }

    protected override void OnDrawFrame() => DrawFrame?.Invoke();
    protected override void OnExit(int exitCode) => Exit?.Invoke(exitCode);
    protected override void OnInit(ScreenBuffer screen) => Init?.Invoke(screen);
    protected override void OnWindowTitleChanged(string title) => WindowTitleChanged?.Invoke(title);
}
