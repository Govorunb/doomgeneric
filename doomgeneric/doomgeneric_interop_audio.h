#include "doomgeneric_audio.h"
#include "doomgeneric_interop.h"

#ifdef FEATURE_SOUND

typedef boolean (*SndInitCallback)();
typedef void (*SndShutdownCallback)();
typedef void (*SndUpdateCallback)();
typedef void (*SndUpdateSoundParamsCallback)(CHANNEL channel, int vol, int sep);
typedef CHANNEL (*SndStartSoundCallback)(sfxdata_t* sound, int channel, int vol, int sep);
typedef void (*SndStopSoundCallback)(CHANNEL channel);
typedef boolean (*SndIsPlayingCallback)(CHANNEL channel);
typedef void (*SndCacheSoundsCallback)(sfxdata_t* sounds, int num_sounds);

typedef boolean (*MusInitCallback)();
typedef void (*MusShutdownCallback)();
typedef void (*MusSetVolumeCallback)(int volume);
typedef void (*MusPauseCallback)();
typedef void (*MusResumeCallback)();
typedef MUS_HANDLE (*MusRegisterSongCallback)(void* data, int len);
typedef void (*MusUnRegisterSongCallback)(MUS_HANDLE handle);
typedef void (*MusPlaySongCallback)(MUS_HANDLE handle, boolean looping);
typedef void (*MusStopSongCallback)();
typedef boolean (*MusIsPlayingCallback)();
typedef void (*MusPollCallback)();

#define OPTIONAL

typedef struct DGSndCallbacks {
	SndInitCallback Init;
	SndShutdownCallback Shutdown;
	SndUpdateCallback Update;
	SndUpdateSoundParamsCallback UpdateSoundParams;
	SndStartSoundCallback StartSound;
	SndStopSoundCallback StopSound;
	SndIsPlayingCallback IsPlaying;
	OPTIONAL SndCacheSoundsCallback CacheSounds;
} dg_snd_callbacks_t;

typedef struct DGMusCallbacks {
	MusInitCallback Init;
	MusShutdownCallback Shutdown;
	MusSetVolumeCallback SetVolume;
	MusPauseCallback Pause;
	MusResumeCallback Resume;
	MusRegisterSongCallback RegisterSong;
	MusUnRegisterSongCallback UnRegisterSong;
	MusPlaySongCallback PlaySong;
	MusStopSongCallback StopSong;
	MusIsPlayingCallback IsPlaying;
	OPTIONAL MusPollCallback Poll;
} dg_mus_callbacks_t;

static dg_snd_callbacks_t* Snd_Callbacks;
static dg_mus_callbacks_t* Mus_Callbacks;

__declspec(dllexport) void SetAudioCallbacks(dg_snd_callbacks_t* callbacks, dg_mus_callbacks_t* mus_callbacks);

#endif