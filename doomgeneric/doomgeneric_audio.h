#include "i_sound.h"
#include "doomfeatures.h"
#include "doomgeneric.h"

#ifdef FEATURE_SOUND

typedef struct sfxdata {
    unsigned short sample_rate;
    unsigned int num_samples;
    short* data; // in 16-bit PCM (signed)
} sfxdata_t;

// arbitrary handle that represents a "channel" internal to the sound module
// note: these are NOT doom engine's "channels" (channel_t) used in e.g. s_sound.c
#define CHANNEL int

boolean DG_Snd_Init(void);
void DG_Snd_Shutdown(void);
// Called periodically to update the subsystem.
void DG_Snd_Update(void);
// Update the sound settings on the given channel.
void DG_Snd_UpdateSoundParams(CHANNEL channel, int vol, int sep);
// Play the given sfx data.
// Returns the channel handle or -1 on failure.
CHANNEL DG_Snd_StartSound(sfxdata_t* data, int channel, int vol, int sep);
void DG_Snd_StopSound(CHANNEL channel);
boolean DG_Snd_IsPlaying(CHANNEL channel);
// (Optional) Called on startup to precache sound effects.
void DG_Snd_CacheSounds(sfxdata_t* sounds, int num_sounds);


typedef struct musdata {
	byte* midi_data;
	int len_bytes;
} musdata_t;

// arbitrary handle for a music track, internal to the music module
#define MUS_HANDLE void*

boolean DG_Mus_Init(void);
void DG_Mus_Shutdown(void);
// range 0-127
void DG_Mus_SetVolume(int volume);
void DG_Mus_Pause(void);
void DG_Mus_Resume(void);
// Register a song handle from data
// Returns a handle that can be used to play the song
MUS_HANDLE DG_Mus_RegisterSong(void* data, int len);
// Un-register (free) song data
void DG_Mus_UnRegisterSong(MUS_HANDLE handle);
void DG_Mus_PlaySong(MUS_HANDLE handle, boolean looping);
void DG_Mus_StopSong(void);
boolean DG_Mus_IsPlaying(void);
// (Optional) Invoked periodically to poll.
void DG_Mus_Poll(void);

#endif