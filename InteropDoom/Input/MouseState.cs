namespace InteropDoom.Input;

public sealed record MouseState
{
    public double DeltaX;
    public double DeltaY;
    public double WheelDelta;
    public MouseButtons Buttons;
}

[Flags]
public enum MouseButtons : byte
{
    None = 0x00,

    Left = 0x01,
    Right = 0x02,
    Middle = 0x04,
    SideBack = 0x08, // XButton1
    SideFront = 0x10, // XButton2
}
