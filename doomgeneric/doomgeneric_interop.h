#include <stdlib.h>
#include <stdint.h>

#define INVOKE(callbacks, func, ...) if (callbacks && callbacks->func) callbacks->func(__VA_ARGS__)

typedef void(*InitCallback)(uint32_t* screen_buffer, int resX, int resY);
typedef void(*DrawFrameCallback)();
typedef void(*SleepCallback)(uint32_t millis);
typedef uint32_t(*GetTicksMillisCallback)();
typedef boolean(*GetKeyCallback)(boolean* pressed, unsigned char* doomKey);
typedef void(*GetMouseCallback)(int* deltax, int* deltay, boolean* left, boolean* right, boolean* middle, int* mwheel);
typedef void(*SetWindowTitleCallback)(const char* title);
typedef void(*ExitCallback)(int exit_code);
typedef void(*LogCallback)(const char* message);

typedef struct DGCallbacks {
	InitCallback Init;
	DrawFrameCallback DrawFrame;
	SleepCallback Sleep;
	GetTicksMillisCallback GetTicksMs;
	GetKeyCallback GetKey;
	GetMouseCallback GetMouse;
	SetWindowTitleCallback SetWindowTitle;
	ExitCallback Exit;
	LogCallback Log;
} dg_callbacks_t;

static dg_callbacks_t* Callbacks;

#define DG_ERROR int

#define DG_SUCCESS 0
#define DG_NO_CALLBACKS 1
#define DG_MALLOC_FAILED -99

__declspec(dllexport) void SetCallbacks(dg_callbacks_t* callbacks);
__declspec(dllexport) void AddIWADPath(const char* dir);
__declspec(dllexport) void SetFallbackIWADPath(const char* dir);
__declspec(dllexport) DG_ERROR Create(int argc, char** argv);
__declspec(dllexport) void Tick();