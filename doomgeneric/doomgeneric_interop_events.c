#include "doomgeneric_interop_events.h"

#define INVOKE_EVENT(event_type, ...) \
if (!EventCallback) return; \
event_##event_type##_t args_ = __VA_ARGS__; \
EventCallback(event_type, &args_);

void DG_OnSecretDiscovered(int count, int total)
{
	INVOKE_EVENT(secret_discovered, { count, total });
}

void DG_OnItemPickedUp(int count, int total)
{
	INVOKE_EVENT(item_pickedup, { count, total });
}

void DG_OnMapEntityKilled(mobj_t* victim, mobj_t* killer)
{
	INVOKE_EVENT(map_entity_killed, { victim, killer });
}

void DG_OnMapEntityDamaged(mobj_t* victim, mobj_t* dealer, int health_dmg, int armor_dmg)
{
	INVOKE_EVENT(map_entity_damaged, { victim, dealer, health_dmg, armor_dmg });
}

void DG_OnLevelCompleted(int episode, int map)
{
	INVOKE_EVENT(level_completed, { episode, map });
}

void DG_OnGameMessage(char* msg)
{
	INVOKE_EVENT(game_message, { msg });
}

void SetEventCallback(dg_event_callback_t* callback)
{
	if (EventCallback) {
		DG_Log("Overwriting existing event callback");
	}
	EventCallback = callback;
}
