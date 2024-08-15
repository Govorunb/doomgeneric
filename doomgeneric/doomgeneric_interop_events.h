#include "doomgeneric_events.h"
#include "doomgeneric_interop.h"

#ifdef FEATURE_EVENTS

typedef enum {
	secret_discovered,
	item_pickedup,
	level_completed,
	map_entity_killed,
	map_entity_damaged,
	game_message,
} event_type_t;

typedef struct {
	int count;
	int total;
} event_secret_discovered_t;

typedef struct {
	int count;
	int total;
} event_item_pickedup_t;

typedef struct {
	int episode;
	int map;
} event_level_completed_t;

typedef struct {
	mobj_t* target;
	mobj_t* attacker;
} event_map_entity_killed_t;

typedef struct {
	mobj_t* target;
	mobj_t* attacker;
	int health_dmg;
	int armor_dmg;
} event_map_entity_damaged_t;

typedef struct {
	char* msg;
} event_game_message_t;

typedef void dg_event_callback_t(event_type_t type, void* data);

static dg_event_callback_t* EventCallback;

__declspec(dllexport) void SetEventCallback(dg_event_callback_t* callback);

#endif