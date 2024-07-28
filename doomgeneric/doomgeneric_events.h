#include "p_mobj.h"
#include "doomgeneric.h"
#include "doomfeatures.h"

#ifdef FEATURE_EVENTS
// The player crossed into a new "secret" special sector
void DG_OnSecretDiscovered(mapsector_t* sector);
// Triggered when an entity dies
// if killed by environment (e.g. crushed by ceiling), killer will be null
void DG_OnKill(mobj_t* target, mobj_t* killer);
// Player just took damage from something
// armor_dmg is the amount absorbed by armor (can be 0), so total damage amount is health_dmg + armor_dmg
// for environmental damage, dealer will be null
void DG_OnPlayerTookDamage(mobj_t* plr, mobj_t* dealer, int health_dmg, int armor_dmg);
// Sent when a map is finished
void DG_OnLevelComplete(int episode, int map);
// Sent when a game message (e.g. "Picked up a clip.") is displayed on screen.
// Also triggers on chat messages in multiplayer.
void DG_OnGameMessage(char* msg);
#endif