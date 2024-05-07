#include <stdio.h>
#include "m_argv.h"
#include "doomgeneric.h"
#include "m_misc.h"

#ifdef _WIN32
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#else
#include <unistd.h>
#endif

uint32_t* DG_ScreenBuffer = 0;

void M_FindResponseFile(void);
void D_DoomMain (void);


void doomgeneric_Create(int argc, char **argv)
{
	// save arguments
    myargc = argc;
    myargv = argv;

	M_FindResponseFile();

	if (!DG_ScreenBuffer)
		DG_ScreenBuffer = malloc(SCREENBUFFER_SIZE);

	DG_Init();

	D_DoomMain ();
}

void Log(const char* format, ...)
{
	char msgbuf[1024];
	va_list argptr = NULL;

	va_start(argptr, format);
	memset(msgbuf, 0, sizeof(msgbuf));
	M_vsnprintf(msgbuf, sizeof(msgbuf), format, argptr);
	va_end(argptr);
	
	DG_Log(msgbuf);
}
