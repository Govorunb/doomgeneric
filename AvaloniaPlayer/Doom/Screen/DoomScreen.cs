using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using WinCursor = System.Windows.Forms.Cursor;

namespace AvaloniaPlayer.Doom.Screen;

public class DoomScreen : UserControl
{
    public IImage Screen { get; } = DoomEngine.Screen.ToAvalonia();

    public DoomScreen()
    {
        Focusable = true;
        Focus();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        DoomEngine.Initialize();
        DoomEngine.OnDrawFrame += Repaint;
        base.OnAttachedToVisualTree(e);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        DoomEngine.OnDrawFrame -= Repaint;
        base.OnDetachedFromVisualTree(e);
    }

    public override void Render(DrawingContext context)
    {
        context.DrawImage(Screen, Bounds);
        base.Render(context);
    }
    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);
        WinCursor.Hide();
        RecenterMouse();
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        WinCursor.Show();
    }

    private void RecenterMouse()
    {
        var screenCenter = App.Current.MainWindow.Position + new PixelPoint((int)Bounds.Center.X, (int)Bounds.Center.Y);
        WinCursor.Position = new(screenCenter.X, screenCenter.Y);
        _ignoreNextMove = true;
    }

    private Point _lastPos;
    private double _mouseSensitivity = 1.5;
    private bool _ignoreNextMove;
    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        Point pos = e.GetCurrentPoint(null).Position;
        Point delta = (pos - _lastPos) * _mouseSensitivity;
        _lastPos = pos;

        bool ignored = _ignoreNextMove;
        _ignoreNextMove = false;
        if (!App.Current.MainWindow.IsActive || !IsFocused || ignored)
            return;

        DoomEngine.OnMouseMove(delta);
        RecenterMouse();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        UpdateMouseButtons(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        UpdateMouseButtons(e);
    }

    private static void UpdateMouseButtons(PointerEventArgs e)
    {
        var p = e.GetCurrentPoint(null).Properties;
        DoomEngine.UpdateMouseButtons(p.IsLeftButtonPressed, p.IsRightButtonPressed, p.IsMiddleButtonPressed);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        DoomEngine.OnKeyEvent(true, e.PhysicalKey);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        DoomEngine.OnKeyEvent(false, e.PhysicalKey);
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        DoomEngine.OnScroll(e.Delta.Y);
    }

    private void Repaint()
    {
        UIThread.Post(InvalidateVisual);
    }
}
