#include <stdlib.h>
#include <stdint.h>
#include <stdbool.h>

typedef void(*InitCallback)(int resX, int resY);
typedef void(*DrawFrameCallback)(uint32_t* screenBuffer);
typedef void(*SleepCallback)(uint32_t millis);
typedef uint32_t(*GetTicksMillisCallback)();
typedef bool(*GetKeyCallback)(bool* pressed, unsigned char* doomKey);
typedef void(*SetWindowTitleCallback)(const char* title);
typedef void(*ExitCallback)(int exit_code);

typedef struct DGCallbacks {
	InitCallback Init;
	DrawFrameCallback DrawFrame;
	SleepCallback Sleep;
	GetTicksMillisCallback GetTicksMs;
	GetKeyCallback GetKey;
	SetWindowTitleCallback SetWindowTitle;
	ExitCallback Exit;
} dg_callbacks_t;

static dg_callbacks_t* Callbacks;

#define DG_ERROR int

#define DG_SUCCESS 0
#define DG_NO_CALLBACKS 1

void SetCallbacks(dg_callbacks_t* callbacks);
DG_ERROR Create(int argc, char** argv);
void Tick();