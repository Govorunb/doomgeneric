#ifndef DOOM_GENERIC
#define DOOM_GENERIC

#include <stdlib.h>
#include <stdint.h>

#define DOOMGENERIC_RESX 640
#define DOOMGENERIC_RESY 360


extern uint32_t* DG_ScreenBuffer;

void doomgeneric_Create(int argc, char **argv);
void doomgeneric_Tick();


//Implement below functions for your platform

// Called when starting up.
void DG_Init();
// Called whenever Doom wants to display screen output.
void DG_DrawFrame();
// Called when there's nothing to do.
void DG_SleepMs(uint32_t ms);
// Return the number of milliseconds since any arbitrary point (just keep it the same, and always tick up).
uint32_t DG_GetTicksMs();
// Return 1 if any processable key was pressed, and 0 otherwise.
// Assign 1 or 0 to pressed.
int DG_GetKey(int* pressed, unsigned char* key);
// Some WADs will set the window title on load.
// This function is optional.
void DG_SetWindowTitle(const char * title);
// Called when the user quits through the in-game interface.
void DG_Exit(int exit_code);

#endif //DOOM_GENERIC
