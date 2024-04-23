#include "doomgeneric.h"
#include "doomgeneric_interop.h"

void DG_Init()
{
	if (!Callbacks->Init) return;
	Callbacks->Init(DOOMGENERIC_RESX, DOOMGENERIC_RESY);
}

void DG_DrawFrame()
{
	if (!Callbacks->DrawFrame) return;
	Callbacks->DrawFrame(DG_ScreenBuffer);
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

void SetCallbacks(dg_callbacks_t* callbacks)
{
	Callbacks = callbacks;
}

int Create(int argc, char** argv)
{
	if (!Callbacks->Init) return DG_NO_CALLBACKS;
	doomgeneric_Create(argc, argv);
}

void Tick()
{
	doomgeneric_Tick();
}