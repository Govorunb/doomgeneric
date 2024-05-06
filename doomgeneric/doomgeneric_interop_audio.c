#include "doomgeneric_interop_audio.h"


boolean DG_Snd_Init() {
	if (!Snd_Callbacks || !Snd_Callbacks->Init) return false;
	return Snd_Callbacks->Init();
}

void DG_Snd_Shutdown() {
	if (!Snd_Callbacks || !Snd_Callbacks->Shutdown) return;
	Snd_Callbacks->Shutdown();
}

void DG_Snd_Update() {
	if (!Snd_Callbacks || !Snd_Callbacks->Update) return;
	Snd_Callbacks->Update();
}

void DG_Snd_UpdateSoundParams(CHANNEL channel, int vol, int sep) {
	if (!Snd_Callbacks || !Snd_Callbacks->UpdateSoundParams) return;
	Snd_Callbacks->UpdateSoundParams(channel, vol, sep);
}

CHANNEL DG_Snd_StartSound(sfxdata_t* sound, int channel, int vol, int sep) {
	if (!Snd_Callbacks || !Snd_Callbacks->StartSound) return -1;
	return Snd_Callbacks->StartSound(sound, channel, vol, sep);
}

void DG_Snd_StopSound(CHANNEL channel) {
	if (!Snd_Callbacks || !Snd_Callbacks->StopSound) return;
	Snd_Callbacks->StopSound(channel);
}

boolean DG_Snd_IsPlaying(CHANNEL channel) {
	if (!Snd_Callbacks || !Snd_Callbacks->IsPlaying) return false;
	return Snd_Callbacks->IsPlaying(channel);
}

void DG_Snd_CacheSounds(sfxdata_t* sounds, int num_sounds) {
	if (!Snd_Callbacks || !Snd_Callbacks->CacheSounds) return;
	Snd_Callbacks->CacheSounds(sounds, num_sounds);
}


boolean DG_Mus_Init() {
	if (!Mus_Callbacks || !Mus_Callbacks->Init) return false;
	return Mus_Callbacks->Init();
}

void DG_Mus_Shutdown() {
	if (!Mus_Callbacks || !Mus_Callbacks->Shutdown) return;
	Mus_Callbacks->Shutdown();
}

void DG_Mus_SetVolume(int volume) {
	if (!Mus_Callbacks || !Mus_Callbacks->SetVolume) return;
	Mus_Callbacks->SetVolume(volume);
}

void DG_Mus_Pause() {
	if (!Mus_Callbacks || !Mus_Callbacks->Pause) return;
	Mus_Callbacks->Pause();
}

void DG_Mus_Resume() {
	if (!Mus_Callbacks || !Mus_Callbacks->Resume) return;
	Mus_Callbacks->Resume();
}

MUS_HANDLE DG_Mus_RegisterSong(void* data, int len) {
	if (!Mus_Callbacks || !Mus_Callbacks->RegisterSong) return NULL;
	return Mus_Callbacks->RegisterSong(data, len);
}

void DG_Mus_UnRegisterSong(MUS_HANDLE handle) {
	if (!Mus_Callbacks || !Mus_Callbacks->UnRegisterSong) return;
	Mus_Callbacks->UnRegisterSong(handle);
}

void DG_Mus_PlaySong(MUS_HANDLE handle, boolean looping) {
	if (!Mus_Callbacks || !Mus_Callbacks->PlaySong) return;
	Mus_Callbacks->PlaySong(handle, looping);
}

void DG_Mus_StopSong() {
	if (!Mus_Callbacks || !Mus_Callbacks->StopSong) return;
	Mus_Callbacks->StopSong();
}

boolean DG_Mus_IsPlaying() {
	if (!Mus_Callbacks || !Mus_Callbacks->IsPlaying) return false;
	return Mus_Callbacks->IsPlaying();
}

void DG_Mus_Poll() {
	if (!Mus_Callbacks || !Mus_Callbacks->Poll) return;
	Mus_Callbacks->Poll();
}

__declspec(dllexport) void SetAudioCallbacks(dg_snd_callbacks_t* snd_callbacks, dg_mus_callbacks_t* mus_callbacks) {
	Snd_Callbacks = snd_callbacks;
	Mus_Callbacks = mus_callbacks;
}