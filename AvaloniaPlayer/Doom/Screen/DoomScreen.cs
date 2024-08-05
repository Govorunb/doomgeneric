using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaPlayer.Doom.Input;
using InteropDoom;
using InteropDoom.Input;
using WinCursor = System.Windows.Forms.Cursor;

namespace AvaloniaPlayer.Doom.Screen;

public class DoomScreen : UserControl
{
    public IImage Screen { get; private set; } = null!;
    public double MouseSensitivity { get; set; } = 2.5;

    public DoomScreen()
    {
        Focusable = true;
        Focus();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        DoomRuntime.Initialize();
        var engine = App.DoomEngine;
        engine.Init += OnDoomInit;
        engine.DrawFrame += Repaint;
        base.OnAttachedToVisualTree(e);
    }

    private void OnDoomInit(ScreenBuffer screen)
    {
        Debug.Assert(screen is { IsInitialized: true });
        Screen = screen.ToAvalonia();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        var engine = App.DoomEngine;
        engine.Init -= OnDoomInit;
        engine.DrawFrame -= Repaint;
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
    private bool _ignoreNextMove;
    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        Point pos = e.GetCurrentPoint(null).Position;
        Point delta = (pos - _lastPos) * MouseSensitivity;
        _lastPos = pos;

        if (!App.Current.MainWindow.IsActive || !IsFocused)
            return;
        if (_ignoreNextMove)
        {
            _ignoreNextMove = false;
            return;
        }

        DoomRuntime.OnMouseMove(delta.X, delta.Y);
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
        DoomRuntime.UpdateMouseButtons(GetMouseButtons(p));
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        DoomRuntime.OnKeyEvent(true, e.PhysicalKey.ToDoomKey());
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        DoomRuntime.OnKeyEvent(false, e.PhysicalKey.ToDoomKey());
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        DoomRuntime.OnMouseScroll(e.Delta.Y);
    }

    private void Repaint()
    {
        UIThread.Post(InvalidateVisual);
    }

    private static MouseButtons GetMouseButtons(PointerPointProperties p)
    {
        MouseButtons buttons = default;
        if (p.IsLeftButtonPressed) buttons |= MouseButtons.Left;
        if (p.IsRightButtonPressed) buttons |= MouseButtons.Right;
        if (p.IsMiddleButtonPressed) buttons |= MouseButtons.Middle;
        if (p.IsXButton1Pressed) buttons |= MouseButtons.SideBack;
        if (p.IsXButton2Pressed) buttons |= MouseButtons.SideFront;
        return buttons;
    }
}
