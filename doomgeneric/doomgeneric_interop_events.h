#include "doomgeneric_events.h"
#include "doomgeneric_interop.h"

#ifdef FEATURE_EVENTS

typedef	void (*SecretDiscoveredCallback)(mapsector_t* sector);
typedef void (*KillCallback)(mobj_t* target, mobj_t* attacker);
typedef void (*PlayerTookDamageCallback)(mobj_t* plr, mobj_t* dealer, int health_dmg, int armor_dmg);
typedef void (*LevelCompleteCallback)(int episode, int map);
typedef void (*GameMessageCallback)(char* msg);
typedef struct {
	SecretDiscoveredCallback OnSecretDiscovered;
	KillCallback OnKill;
	PlayerTookDamageCallback OnPlayerTookDamage;
	LevelCompleteCallback OnLevelComplete;
	GameMessageCallback OnGameMessage;
} dg_event_callbacks_t;

static dg_event_callbacks_t* EventCallbacks;

__declspec(dllexport) void SetEventCallbacks(dg_event_callbacks_t* callbacks);

#endif