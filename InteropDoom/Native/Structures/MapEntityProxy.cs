using InteropDoom.Native.Definitions;
using System.Numerics;
using System.Runtime.InteropServices;

namespace InteropDoom.Native.Structures;

/// <summary>
/// A proxy for the <c>mobj_t</c> native struct with partial read-only access to some commonly wanted/useful members.
/// </summary>
/// <remarks>
/// See p_mobj.h in the doomgeneric project for the full definition (look up "<c>typedef struct mobj_s</c>").
/// </remarks>
public unsafe readonly struct MapEntityProxy : IEquatable<MapEntityProxy>
{
    private readonly nint _ptr;

    // field offsets are 0-indexed
    // they are also not exactly "field" indices but more like "primitive component" indices
    internal const string LayoutString
        = "ppp" //  0 - thinker struct (thinker* prev, thinker* next, func* function)
        + "iii" //  3 - position x,y,z
        + "pp"  //  6 - snext, sprev
        + "iii" //  8 - angle, sprite, frame
        + "ppp" // 11 - bnext, bprev, subsector
        + "ii"  // 14 - floorz, ceilingz
        + "ii"  // 16 - radius, height (collision cylinder for movement)
        + "iii" // 18 - velocity x,y,z
        + "i"   // 21 - validcount
        + "i"   // 22 - type
        + "pipi" // 23 - info, tics, state, flags
        + "i"   // 27 - health
        + "ii"  // 28 - movedir, movecount
        + "p"   // 30 - target
        + "ii"  // 31 - reactiontime, threshold
        + "p"   // 33 - player
        + "i"   // 34 - lastlook
        + "sssss" // 35 - spawnpoint mapthing_t struct (short x, short y, short angle, short type, short options)
        + "p"   // 40 - tracer
        ;
    private static readonly StructLayoutHelper _layout = new(LayoutString);

    private static readonly int PositionOffset = _layout.Offsets[3];
    private static readonly int VelocityOffset = _layout.Offsets[18];
    private static readonly int TypeOffset = _layout.Offsets[22];
    private static readonly int InfoOffset = _layout.Offsets[23];
    private static readonly int HealthOffset = _layout.Offsets[27];
    private static readonly int TargetOffset = _layout.Offsets[30];
    private static readonly int PlayerOffset = _layout.Offsets[33];

    internal MapEntityProxy(nint ptr)
    {
        _ptr = ptr;
    }

    public readonly Vector3 Position => ReadVector3(PositionOffset);
    public readonly Vector3 Velocity => ReadVector3(VelocityOffset);
    public readonly MapEntityType Type => (MapEntityType)ReadInt32(TypeOffset);
    // todo
    // public readonly MapEntityInfoProxy Info => new(ReadPtr(InfoOffset));
    public readonly int Health => ReadInt32(HealthOffset);
    public readonly MapEntityProxy Target => new(ReadPtr(TargetOffset));
    public readonly bool IsPlayer => Type == MapEntityType.MT_PLAYER;
    // todo player proxy
    // public readonly PlayerProxy Player => new(ReadPtr(PlayerOffset));

    private void CheckInitialized()
    {
        if (_ptr == default)
            throw new InvalidOperationException($"Unititialized {nameof(MapEntityProxy)}");
    }

    private int ReadInt32(int offset)
    {
        CheckInitialized();
        return Marshal.ReadInt32(_ptr, offset);
    }

    private nint ReadPtr(int offset)
    {
        CheckInitialized();
        return Marshal.ReadIntPtr(_ptr, offset);
    }
    private Vector3 ReadVector3(int offset)
    {
        CheckInitialized();
        ReadOnlySpan<int> raw = new((int*)(_ptr + offset), 3);
        return new(
            FixedPoint.ToFloat(raw[0]),
            FixedPoint.ToFloat(raw[1]),
            FixedPoint.ToFloat(raw[2])
        );
    }

    public bool Equals(MapEntityProxy other) => _ptr == other._ptr;
    public override bool Equals(object? obj) => obj is MapEntityProxy proxy && Equals(proxy);

    public static bool operator ==(MapEntityProxy left, MapEntityProxy right) => left.Equals(right);
    public static bool operator !=(MapEntityProxy left, MapEntityProxy right) => !left.Equals(right);

    public static implicit operator bool(MapEntityProxy proxy) => proxy._ptr != default;

    public override int GetHashCode() => (int)_ptr;
}
