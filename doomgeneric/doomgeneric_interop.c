#include "doomgeneric.h"
#include "m_argv.h"
#include "doomgeneric_interop.h"
#include <string.h>

#define INVOKE_CALLBACK(func, ...) INVOKE(Callbacks, func, __VA_ARGS__)
#define INVOKE_CALLBACK_RETURNS(func, def_val, ...) (Callbacks && Callbacks->func ? Callbacks->func(__VA_ARGS__) : def_val)

void DG_Init()
{
	INVOKE_CALLBACK(Init, DG_ScreenBuffer, DOOMGENERIC_RESX, DOOMGENERIC_RESY);
}

void DG_DrawFrame()
{
	INVOKE_CALLBACK(DrawFrame);
}

void DG_SleepMs(uint32_t ms)
{
	INVOKE_CALLBACK(Sleep, ms);
}

uint32_t DG_GetTicksMs()
{
	return INVOKE_CALLBACK_RETURNS(GetTicksMs, 0);
}

int DG_GetKey(int* pressed, unsigned char* doomKey)
{
	return INVOKE_CALLBACK_RETURNS(GetKey, 0, (boolean*)pressed, doomKey);
}

void DG_GetMouse(int* deltax, int* deltay, int* left, int* right, int* middle, int* mwheel)
{
	INVOKE_CALLBACK(GetMouse, deltax, deltay, (boolean*)left, (boolean*)right, (boolean*)middle, mwheel);
}

void DG_SetWindowTitle(const char* title)
{
	INVOKE_CALLBACK(SetWindowTitle, title);
}

void DG_Exit(int exit_code)
{
	INVOKE_CALLBACK(Exit, exit_code);
}

void DG_Log(const char* message)
{
	INVOKE_CALLBACK(Log, message);
}

__declspec(dllexport) void SetCallbacks(dg_callbacks_t* callbacks)
{
	if (Callbacks) {
		DG_Log("Overwriting existing callbacks");
		// notify the new logger as well (though it's debatable whether it should care)
		INVOKE(callbacks, Log, "Overwrote existing callbacks");
	}
	Callbacks = callbacks;
}

// don't rely on managed caller to pin argv
static char** stored_argv = NULL;
__declspec(dllexport) int Create(int argc, char** argv)
{
	if (!Callbacks || !Callbacks->Init) return DG_NO_CALLBACKS;

	stored_argv = malloc(argc * sizeof(char*));
	if (!stored_argv) return DG_MALLOC_FAILED;

	for (int i = 0; i < argc; i++)
	{
		char* arg = malloc(strlen(argv[i]) + 1);
		if (!arg)
		{
			for (int j = 0; j < i; j++)
			{
				free(stored_argv[j]);
			}
			return DG_MALLOC_FAILED;
		}

		strcpy(arg, argv[i]);
		stored_argv[i] = arg;
	}
	doomgeneric_Create(argc, stored_argv);
	return DG_SUCCESS;
}

__declspec(dllexport) void AddIWADPath(const char* path)
{
	doomgeneric_AddIWADPath(strdup(path));
}
__declspec(dllexport) void SetFallbackIWADPath(const char* path)
{
	doomgeneric_SetFallbackIWADPath(strdup(path));
}

__declspec(dllexport) void Tick()
{
	doomgeneric_Tick();
}