#ifndef DOOM_GENERIC
#define DOOM_GENERIC

#include <stdlib.h>
#include <stdint.h>

#define DOOMGENERIC_RESX 640
#define DOOMGENERIC_RESY 400

#define SCREENBUFFER_SIZE (DOOMGENERIC_RESX * DOOMGENERIC_RESY * sizeof(uint32_t))

extern uint32_t* DG_ScreenBuffer;

void doomgeneric_Create(int argc, char **argv);
void doomgeneric_AddIWADPath(char* dir);
void doomgeneric_SetFallbackIWADPath(char* dir);
void doomgeneric_Tick();


//Implement below functions for your platform

// Called when starting up.
void DG_Init();
// Called whenever Doom wants to display screen output.
void DG_DrawFrame();
// Called when Doom needs to pass time. You are expected to return *after* the time has passed.
// If you're embedding Doom in another application, you'll need to run the callbacks on a separate thread.
void DG_SleepMs(uint32_t ms);
// Return the number of milliseconds since some arbitrary point, most commonly application startup.
uint32_t DG_GetTicksMs();
// Return 1 if any processable key was pressed, and 0 otherwise.
// Assign 1 or 0 to pressed.
int DG_GetKey(int* pressed, unsigned char* key);
// For deltax and deltay: return the difference since the last call.
// For left, right, and middle: 0/1 if (not) currently pressed.
// For the mouse wheel - positive for up, 0 for none (not scrolling), negative for down.
void DG_GetMouse(int* deltax, int* deltay, int* left, int* right, int* middle, int* mwheel);
// Some WADs will set the window title on load.
// This function is optional.
void DG_SetWindowTitle(const char* title);
// Called when the user quits through the in-game interface.
void DG_Exit(int exit_code);
// Optional log function for debugging.
void DG_Log(const char* message);

// Helper for logging formatted messages.
// It's used internally so you don't need to implement this - it calls DG_Log.
void Log(const char* format, ...);

#endif //DOOM_GENERIC
