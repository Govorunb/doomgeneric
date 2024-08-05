using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging;
using Avalonia.Markup.Xaml;
using AvaloniaPlayer.Doom;
using InteropDoom;

namespace AvaloniaPlayer;
public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static new App Current => (App)Application.Current!;
    internal static AvaloniaDoomEngine DoomEngine { get; } = new();

    public MainWindow MainWindow { get; private set; } = null!;

    public override void OnFrameworkInitializationCompleted()
    {
        DoomRuntime.Engine = DoomEngine;
        DoomEngine.Exit += (code) =>
        {
            UIThread.Post(() =>
            {
                Logger.TryGet(code != 0 ? LogEventLevel.Error : LogEventLevel.Information, "DOOM")
                    ?.Log(this, $"Doom exited with code {code}, quitting");
                MainWindow.Close();
            });
        };
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = MainWindow = new MainWindow();
            DoomEngine.WindowTitleChanged += (title) => UIThread.Post(() => MainWindow.Title = title);
        }

        base.OnFrameworkInitializationCompleted();
    }
}