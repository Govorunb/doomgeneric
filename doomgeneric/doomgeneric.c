#include <stdio.h>

#include "m_argv.h"

#include "doomgeneric.h"

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

