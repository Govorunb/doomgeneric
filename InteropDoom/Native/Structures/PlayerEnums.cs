namespace InteropDoom.Native.Structures;

public enum PlayerState
{
    Alive,
    Dead,
    Respawning,
}

public enum ArmorType
{
    None,  // no armor
    Green, // absorbs 1/3 damage
    Blue,  // absorbs 1/2 damage
}

public enum PowerupType
{
    Invulnerability,
    Berserk,
    PartialInvisibility,
    RadiationProtectionSuit,
    ComputerMap,
    Infrared,
}

public enum KeycardType
{
    BlueCard,
    YellowCard,
    RedCard,
    BlueSkull,
    YellowSkull,
    RedSkull,
}

public enum WeaponType
{
    Fist,
    Pistol,
    Shotgun,
    Chaingun,
    Missile,
    Plasma,
    BFG,
    Chainsaw,
    SuperShotgun
}

public enum AmmoType
{
    Clip,    // pistol/chaingun
    Shell,   // shotgun/dbl shotgun
    Cell,    // plasmagun/bfg
    Missile, // rocket launcher
}

[Flags]
public enum Cheats
{
    None = 0,
    Noclip = 1,
    GodMode = 2,
    NoMomentum = 4, // impossible to gain in-game
}