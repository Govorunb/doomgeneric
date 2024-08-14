#include "doomgeneric.h"
#include "doomfeatures.h"
#include "p_mobj.h"
#include "r_defs.h"

#ifdef FEATURE_EVENTS
// The player crossed into a new "secret" special sector
void DG_OnSecretDiscovered(sector_t* sector);
// Triggered when an entity dies
// if killed by environment (e.g. crushed by ceiling), killer will be null
void DG_OnMapEntityKilled(mobj_t* victim, mobj_t* killer);
// Map entity just took damage from something
// armor_dmg is the amount absorbed by armor (can be 0, always 0 for non-players), so total damage amount is health_dmg + armor_dmg
// for environmental damage (floor, ceiling, etc), dealer will be null
void DG_OnMapEntityDamaged(mobj_t* victim, mobj_t* dealer, int health_dmg, int armor_dmg);
// Sent when a map is finished
void DG_OnLevelCompleted(int episode, int map);
// Sent when a game message (e.g. "Picked up a clip.") is displayed on screen.
// Also triggers on chat messages in multiplayer.
void DG_OnGameMessage(char* msg);
#endif