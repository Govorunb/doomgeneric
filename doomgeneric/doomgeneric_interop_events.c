#include "doomgeneric_interop_events.h"

#define INVOKE_EVENT(func, ...) INVOKE(EventCallbacks, func, __VA_ARGS__)

void DG_OnSecretDiscovered(mapsector_t* sector)
{
	INVOKE_EVENT(OnSecretDiscovered, sector);
}

void DG_OnKill(mobj_t* target, mobj_t* killer)
{
	INVOKE_EVENT(OnKill, target, killer);
}

void DG_OnPlayerTookDamage(mobj_t* plr, mobj_t* dealer, int health_dmg, int armor_dmg)
{
	INVOKE_EVENT(OnPlayerTookDamage, plr, dealer, health_dmg, armor_dmg);
}

void DG_OnLevelComplete(int episode, int map)
{
	INVOKE_EVENT(OnLevelComplete, episode, map);
}

void DG_OnGameMessage(char* msg)
{
	INVOKE_EVENT(OnGameMessage, msg);
}

void SetEventCallbacks(dg_event_callbacks_t* callbacks)
{
	if (EventCallbacks) {
		DG_Log("Overwriting existing callbacks");
	}
	EventCallbacks = callbacks;
}
