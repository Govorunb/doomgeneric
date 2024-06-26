#include "doomgeneric.h"
#include "m_argv.h"
#include "doomgeneric_interop.h"
#include <string.h>

void DG_Init()
{
	if (!Callbacks || !Callbacks->Init) return;
	Callbacks->Init(DOOMGENERIC_RESX, DOOMGENERIC_RESY);
}

void DG_DrawFrame()
{
	if (!Callbacks || !Callbacks->DrawFrame) return;
	Callbacks->DrawFrame((unsigned char *)DG_ScreenBuffer, SCREENBUFFER_SIZE);
}

void DG_SleepMs(uint32_t ms)
{
	if (!Callbacks || !Callbacks->Sleep) return;
	Callbacks->Sleep(ms);
}

uint32_t DG_GetTicksMs()
{
	if (!Callbacks || !Callbacks->GetTicksMs) return 0;
	return Callbacks->GetTicksMs();
}

int DG_GetKey(int* pressed, unsigned char* doomKey)
{
	if (!Callbacks || !Callbacks->GetKey) return 0;
	return Callbacks->GetKey(pressed, doomKey);
}

void DG_GetMouse(int* deltax, int* deltay, int* left, int* right, int* middle, int* mwheel)
{
	if (!Callbacks || !Callbacks->GetMouse) return;
	Callbacks->GetMouse(deltax, deltay, left, right, middle, mwheel);
}

void DG_SetWindowTitle(const char* title)
{
	if (!Callbacks || !Callbacks->SetWindowTitle) return;
	Callbacks->SetWindowTitle(title);
}

void DG_Exit(int exit_code)
{
	if (!Callbacks || !Callbacks->Exit) return;
	Callbacks->Exit(exit_code);
}

void DG_Log(const char* message)
{
	if (!Callbacks || !Callbacks->Log) return;
	Callbacks->Log(message);
}

__declspec(dllexport) void SetCallbacks(dg_callbacks_t* callbacks)
{
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