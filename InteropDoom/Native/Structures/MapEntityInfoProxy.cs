using InteropDoom.Native.Definitions;
using InteropDoom.Utilities;

namespace InteropDoom.Native.Structures;

public readonly struct MapEntityInfoProxy
{
    private readonly nint _ptr;
    internal MapEntityInfoProxy(nint ptr) => _ptr = ptr;

    internal const string LayoutString
        = "iii" //  0 - doomednum, spawnstate, spawnhealth
        + "ii"  //  3 - seestate, seesound
        + "ii"  //  5 - reactiontime, attacksound
        + "iii" //  7 - painstate, painchance, painsound
        + "ii"  // 10 - meleestate, missilestate
        + "iii" // 12 - deathstate, xdeathstate, deathsound
        + "i"   // 15 - speed
        + "iii" // 16 - radius, height, mass (collision cylinder)
        + "i"   // 19 - damage
        + "i"   // 20 - activesound
        + "i"   // 21 - flags
        + "i"   // 22 - raisestate
        ;
    private static readonly StructLayoutHelper _layout = new(LayoutString);

    private static readonly int SpawnHealthOffset = _layout.Offsets[2];
    private static readonly int FlinchChanceOffset = _layout.Offsets[8];
    private static readonly int SpeedOffset = _layout.Offsets[15];
    private static readonly int DamageOffset = _layout.Offsets[19];

    public int SpawnHealth => _ptr.ReadInt32(SpawnHealthOffset);
    /// <summary>
    /// The chance that the entity will flinch (interrupt animation and play pain sound/anim) when hit.
    /// </summary>
    public float FlinchChance => _ptr.ReadInt32(FlinchChanceOffset) / 255f;
    public float Speed => FixedPoint.ToFloat(_ptr.ReadInt32(SpeedOffset));
    /// <summary>
    /// Applicable only to missile entities (and Lost Souls).
    /// </summary>
    public int Damage => _ptr.ReadInt32(DamageOffset);

    public bool Equals(MapEntityInfoProxy other) => _ptr == other._ptr;
    public override bool Equals(object? obj) => obj is MapEntityInfoProxy proxy && Equals(proxy);

    public static bool operator ==(MapEntityInfoProxy left, MapEntityInfoProxy right) => left.Equals(right);
    public static bool operator !=(MapEntityInfoProxy left, MapEntityInfoProxy right) => !left.Equals(right);

    public static implicit operator bool(MapEntityInfoProxy proxy) => proxy._ptr != default;

    public override int GetHashCode() => (int)_ptr;
}
