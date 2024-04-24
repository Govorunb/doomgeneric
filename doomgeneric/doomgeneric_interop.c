#include "doomgeneric.h"
#include "m_argv.h"
#include "doomgeneric_interop.h"
#include <string.h>

void DG_Init()
{
	if (!Callbacks->Init) return;
	Callbacks->Init(DOOMGENERIC_RESX, DOOMGENERIC_RESY);
}

void DG_DrawFrame()
{
	if (!Callbacks->DrawFrame) return;
	Callbacks->DrawFrame(DG_ScreenBuffer, SCREENBUFFER_SIZE);
}

void DG_SleepMs(uint32_t ms)
{
	if (!Callbacks->Sleep) return;
	Callbacks->Sleep(ms);
}

uint32_t DG_GetTicksMs()
{
	if (!Callbacks->GetTicksMs) return 0;
	return Callbacks->GetTicksMs();
}

int DG_GetKey(int* pressed, unsigned char* doomKey)
{
	if (!Callbacks->GetKey) return 0;
	return Callbacks->GetKey(pressed, doomKey);
}

void DG_SetWindowTitle(const char* title)
{
	if (!Callbacks->SetWindowTitle) return;
	Callbacks->SetWindowTitle(title);
}

void DG_Exit(int exit_code)
{
	if (!Callbacks->Exit) return;
	Callbacks->Exit(exit_code);
}

void DG_Log(const char* message)
{
	if (!Callbacks->Log) return;
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
	if (!Callbacks->Init) return DG_NO_CALLBACKS;

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

__declspec(dllexport) void Tick()
{
	doomgeneric_Tick();
}