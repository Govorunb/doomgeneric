#include "doomgeneric_interop_audio.h"

#define INVOKE_SND(func, ...) if (Snd_Callbacks && Snd_Callbacks->func) Snd_Callbacks->func(__VA_ARGS__)
#define INVOKE_SND_RETURNS(func, def_val, ...) (Snd_Callbacks && Snd_Callbacks->func ? Snd_Callbacks->func(__VA_ARGS__) : def_val)
#define INVOKE_MUS(func, ...) if (Mus_Callbacks && Mus_Callbacks->func) Mus_Callbacks->func(__VA_ARGS__)
#define INVOKE_MUS_RETURNS(func, def_val, ...) (Mus_Callbacks && Mus_Callbacks->func ? Mus_Callbacks->func(__VA_ARGS__) : def_val)

boolean DG_Snd_Init() {
	return INVOKE_SND_RETURNS(Init, false);
}

void DG_Snd_Shutdown() {
	INVOKE_SND(Shutdown);
}

void DG_Snd_Update() {
	INVOKE_SND(Update);
}

void DG_Snd_UpdateSoundParams(CHANNEL channel, int vol, int sep) {
	INVOKE_SND(UpdateSoundParams, channel, vol, sep);
}

CHANNEL DG_Snd_StartSound(sfxdata_t* sound, int channel, int vol, int sep) {
	return INVOKE_SND_RETURNS(StartSound, -1, sound, channel, vol, sep);
}

void DG_Snd_StopSound(CHANNEL channel) {
	INVOKE_SND(StopSound, channel);
}

boolean DG_Snd_IsPlaying(CHANNEL channel) {
	return INVOKE_SND_RETURNS(IsPlaying, false, channel);
}

void DG_Snd_CacheSounds(sfxdata_t* sounds, int num_sounds) {
	INVOKE_SND(CacheSounds, sounds, num_sounds);
}


boolean DG_Mus_Init() {
	return INVOKE_MUS_RETURNS(Init, false);
}

void DG_Mus_Shutdown() {
	INVOKE_MUS(Shutdown);
}

void DG_Mus_SetVolume(int volume) {
	INVOKE_MUS(SetVolume, volume);
}

void DG_Mus_Pause() {
	INVOKE_MUS(Pause);
}

void DG_Mus_Resume() {
	INVOKE_MUS(Resume);
}

MUS_HANDLE DG_Mus_RegisterSong(void* data, int len) {
	return INVOKE_MUS_RETURNS(RegisterSong, NULL, data, len);
}

void DG_Mus_UnRegisterSong(MUS_HANDLE handle) {
	INVOKE_MUS(UnRegisterSong, handle);
}

void DG_Mus_PlaySong(MUS_HANDLE handle, boolean looping) {
	INVOKE_MUS(PlaySong, handle, looping);
}

void DG_Mus_StopSong() {
	INVOKE_MUS(StopSong);
}

boolean DG_Mus_IsPlaying() {
	return INVOKE_MUS_RETURNS(IsPlaying, false);
}

void DG_Mus_Poll() {
	INVOKE_MUS(Poll);
}

__declspec(dllexport) void SetAudioCallbacks(dg_snd_callbacks_t* snd_callbacks, dg_mus_callbacks_t* mus_callbacks) {
	if (snd_callbacks) {
		DG_Log("Overwriting existing snd_callbacks");
	}
	Snd_Callbacks = snd_callbacks;
	if (mus_callbacks) {
		DG_Log("Overwriting existing mus_callbacks");
	}
	Mus_Callbacks = mus_callbacks;
}