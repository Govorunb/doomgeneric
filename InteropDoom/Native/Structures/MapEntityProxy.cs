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

    private const string LayoutString
        = "ppp" // thinker struct (thinker* prev, thinker* next, func* function)
        + "iii" // position x,y,z
        + "pp" // snext, sprev
        + "iii" // angle, sprite, frame
        + "ppp" // bnext, bprev, subsector
        + "ii" // floorz, ceilingz
        + "ii" // radius, height (collision cylinder for movement)
        + "iii" // velocity x,y,z
        + "i" // validcount
        + "i" // type
        + "pipi" // info, tics, state, flags
        + "i" // health
        + "ii" // movedir, movecount
        + "p" // target
        + "ii" // reactiontime, threshold
        + "p" // player
        + "i" // lastlook
        + "sssss" // spawnpoint mapthing_t struct (short x, short y, short angle, short type, short options)
        + "p" // tracer
        ;
    private static readonly StructLayoutHelper _layout = new(LayoutString);
    
    // field offsets are 0-indexed
    // they are also not exactly "field" indices but more like "primitive component" indices
    private const int PositionOffset = 3;
    private const int VelocityOffset = 18;
    private const int TypeOffset = 22;
    private const int HealthOffset = 27;
    private const int TargetOffset = 30;
    private const int PlayerOffset = 33;

    internal MapEntityProxy(nint ptr)
    {
        _ptr = ptr;
    }

    public readonly Vector3 Position => ReadVector3(PositionOffset);
    public readonly Vector3 Velocity => ReadVector3(VelocityOffset);
    public readonly MapEntityType Type => (MapEntityType)ReadInt32(TypeOffset);
    public readonly int Health => ReadInt32(HealthOffset);
    public readonly MapEntityProxy Target => new(ReadPtr(TargetOffset));
    public readonly bool IsPlayer => Type == MapEntityType.MT_PLAYER;
    // todo player proxy
    // public readonly PlayerProxy Player => new(ReadPtr(PlayerOffset));

    private void CheckInitialized()
    {
        if (_ptr == default) throw new InvalidOperationException($"Unititialized {nameof(MapEntityProxy)}");
    }

    private int ReadInt32(int field)
    {
        CheckInitialized();
        return Marshal.ReadInt32(_ptr, _layout.Offsets[field]);
    }

    private nint ReadPtr(int field)
    {
        CheckInitialized();
        return Marshal.ReadIntPtr(_ptr, _layout.Offsets[field]);
    }
    private Vector3 ReadVector3(int field)
    {
        CheckInitialized();
        ReadOnlySpan<int> raw = new((int*)(_ptr + _layout.Offsets[field]), 3);
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
