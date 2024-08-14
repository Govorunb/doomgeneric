namespace InteropDoom.Native.Definitions;

internal readonly struct FixedPoint(int val)
{
    private readonly int _value = val;

    public const int FRACBITS = 16;
    public const int FRACUNIT = 1 << FRACBITS;

    public static implicit operator FixedPoint(int value) => new(value);
    public static implicit operator int(FixedPoint fp) => fp._value;

    public static implicit operator float(FixedPoint fp) => ToFloat(fp);

    public static FixedPoint operator +(FixedPoint a, FixedPoint b) => new(a._value + b._value);
    public static FixedPoint operator -(FixedPoint a, FixedPoint b) => new(a._value - b._value);
    public static FixedPoint operator *(FixedPoint a, FixedPoint b) => new((int)((long)a * b >> FRACBITS));
    public static FixedPoint operator /(FixedPoint a, FixedPoint b)
    {
        if ((Math.Abs(a) >> 14) >= Math.Abs(b))
            return (a ^ b) < 0 ? int.MinValue : int.MaxValue;
        return (int)(((long)a << FRACBITS) / b);
    }

    public static float ToFloat(FixedPoint fp) => ToFloat(fp._value);
    public static float ToFloat(int value)
    {
        float sign = 1;
        if (value < 0)
        {
            // flip two's complement
            value = ~value;
            value += 1;
            sign = -1;
        }
        return sign * value / FRACUNIT;
    }
}
