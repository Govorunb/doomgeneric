using InteropDoom.Utilities;

namespace InteropDoom.Native.Structures;
/// <summary>
/// See player_s/player_t in "d_player.h" in the doomgeneric project
/// </summary>
public readonly struct PlayerProxy
{
    public readonly nint _ptr;
    internal PlayerProxy(nint ptr) => _ptr = ptr;

    internal const string LayoutString
        = "pi"     //  0 - mo (mobj_t*), playerstate (enum)
        + "bbsbbbbibb" // 2 - ticcmd_t (not writing all of it out here, cba)
        + "iiii"   // 12 - viewz, viewheight, deltaviewheight, bob
        + "iii"    // 16 - health, armorpoints, armortype
        + "iiiiii" // 19 - powers[NUMPOWERS] (6 tic counters)
        // int32 booleans, what a waste
        + "iiiiii" // 25 - cards[NUMCARDS] (keycards/skullkeys) (BYR, cards then skulls)
        + "i"      // 31 - backpack
        + "iiii"   // 32 - frags[MAXPLAYERS] (4)
        + "ii"     // 36 - readyweapon, pendingweapon
        + "iiiiiiiii" // 38 - weaponowned[NUMWEAPONS] (9)
        + "iiii"   // 47 - ammo[NUMAMMO] (4)
        + "iiii"   // 51 - maxammo[NUMAMMO] (4)
        + "ii"     // 55 - attackdown, usedown
        + "ii"     // 57 - cheats (flags), refire
        + "iii"    // 59 - killcount, itemcount, secretcount
        + "p"      // 62 - message (char*)
        + "ii"     // 63 - damagecount, bonuscount
        + "p"      // 65 - attacker (mobj_t*)
        + "iii"    // 66 - extralight, fixedcolormap, colormap
        + "piiipiii" // 69 - pspdef_t psprites[NUMSPRITES] (2); pspdef_t is (state_t* state, int tics, fixed_t sx, fixed_t sy)
        + "i"      // 77 - didsecret
        ;

    private static readonly StructLayoutHelper _layout = new(LayoutString);

    private static readonly int MapEntityOffset = _layout.Offsets[0];
    private static readonly int StateOffset = _layout.Offsets[1];
    private static readonly int HealthOffset = _layout.Offsets[16];
    private static readonly int ArmorOffset = _layout.Offsets[17];
    private static readonly int ArmorTypeOffset = _layout.Offsets[18];
    private static readonly int PowerupsOffset = _layout.Offsets[19];
    private static readonly int KeycardsOffset = _layout.Offsets[25];
    private static readonly int BackpackOffset = _layout.Offsets[31];
    private static readonly int WeaponOffset = _layout.Offsets[36];
    private static readonly int WeaponOwnedOffset = _layout.Offsets[38];
    private static readonly int AmmoOffset = _layout.Offsets[47];
    private static readonly int MaxAmmoOffset = _layout.Offsets[51];
    private static readonly int KillCountOffset = _layout.Offsets[59];
    private static readonly int ItemCountOffset = _layout.Offsets[60];
    private static readonly int SecretCountOffset = _layout.Offsets[61];

    public MapEntityProxy MapEntity => new(_ptr.ReadPtr(MapEntityOffset));
    public PlayerState State => (PlayerState)_ptr.ReadInt32(StateOffset);
    public int SavedHealth => _ptr.ReadInt32(HealthOffset);
    public int SavedArmor => _ptr.ReadInt32(ArmorOffset);
    public ArmorType ArmorType => (ArmorType)_ptr.ReadInt32(ArmorTypeOffset);

    public bool IsPowerupActive(PowerupType powerupType) => _ptr.ReadArray<int>(PowerupsOffset, (int)powerupType) > 0;
    public float GetRemainingPowerupTime(PowerupType powerupType) => _ptr.ReadArray<int>(PowerupsOffset, (int)powerupType) / 35f; // tics per second
    public bool HasKeycard(KeycardType keycardType) => _ptr.ReadArray<int>(KeycardsOffset, (int)keycardType) != 0;
    public bool HasAmmoBackpack => _ptr.ReadArray<int>(BackpackOffset, 0) != 0;
    public WeaponType CurrentWeapon => (WeaponType)_ptr.ReadInt32(WeaponOffset);
    public bool HasWeapon(WeaponType weaponType) => _ptr.ReadArray<int>(WeaponOwnedOffset, (int)weaponType) != 0;
    public float GetCurrentAmmo(AmmoType ammoType) => _ptr.ReadArray<int>(AmmoOffset, (int)ammoType);
    public float GetMaxAmmo(AmmoType ammoType) => _ptr.ReadArray<int>(MaxAmmoOffset, (int)ammoType);
    public (int Kills, int Items, int Secrets) LevelStats => new(_ptr.ReadInt32(KillCountOffset), _ptr.ReadInt32(ItemCountOffset), _ptr.ReadInt32(SecretCountOffset));

    public bool Equals(PlayerProxy other) => _ptr == other._ptr;
    public override bool Equals(object? obj) => obj is PlayerProxy proxy && Equals(proxy);

    public static bool operator ==(PlayerProxy left, PlayerProxy right) => left.Equals(right);
    public static bool operator !=(PlayerProxy left, PlayerProxy right) => !left.Equals(right);

    public static implicit operator bool(PlayerProxy proxy) => proxy._ptr != default;

    public override int GetHashCode() => (int)_ptr;
}
