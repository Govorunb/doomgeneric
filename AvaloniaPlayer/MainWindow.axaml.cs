using Avalonia.Controls;
using AvaloniaPlayer.Doom;

namespace AvaloniaPlayer;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DoomEngine.WindowTitleChanged += () => UIThread.Post(() => Title = DoomEngine.WindowTitle);
    }
}