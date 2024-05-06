#include <string.h>
#include "doomgeneric_audio.h"
#include "m_misc.h"
#include "w_wad.h"
#include "z_zone.h"
#include "memio.h"
#include "i_system.h"
#include "mus2mid.h"

#ifdef FEATURE_SOUND
static snddevice_t sound_devices[] = {SNDDEVICE_SB};
static boolean _use_sfx_prefix;
static musdata_t current_song = { NULL, 0 };

static boolean _DG_Snd_Init(boolean use_sfx_prefix)
{
	_use_sfx_prefix = use_sfx_prefix;
    return DG_Snd_Init();
}
static int _DG_Snd_GetSfxLumpNum(sfxinfo_t* sfx)
{
    // adapted from i_sdlsound.c
    char namebuf[11]; // was 9 even though sfx->name is 9 and there's a 2ch prefix

    // Linked sfx lumps? Get the lump number for the sound linked to.
    while (sfx->link != NULL)
        sfx = sfx->link;

    // Doom adds a DS* prefix to sound lumps.
    if (_use_sfx_prefix)
    {
        M_snprintf(namebuf, arrlen(namebuf), "ds%s", sfx->name);
    }
    else
    {
        M_StringCopy(namebuf, sfx->name, arrlen(namebuf));
    }

    return W_GetNumForName(namebuf);
}

// expand sample data from u8 to s16
static short* ExpandSoundData(byte* samples, unsigned int num_samples) {
    uint32_t expanded_length = num_samples * 2;
    short* expanded = (short*)Z_Malloc(expanded_length, PU_STATIC, 0);

    for (unsigned int i = 0; i < num_samples; i++) {
        expanded[i] = (samples[i] << 8) - 0x8000;
    }
    return expanded;
}

static void LoadSound(sfxinfo_t* sfxinfo) {
    if (!sfxinfo) {
        return;
    }
    if (sfxinfo->driver_data) {
		Z_Free(((sfxdata_t*)sfxinfo->driver_data)->data);
        Z_Free(sfxinfo->driver_data);
    }

    int lumpnum = sfxinfo->lumpnum;
    byte* data = (byte*)W_CacheLumpNum(lumpnum, PU_STATIC);
    unsigned int lumplen = W_LumpLength(lumpnum);

    // header is 8 bytes
    if (lumplen < 8 || data[0] != 0x03 || data[1] != 0x00) {
        return;
    }

	sfxdata_t* sfx = (sfxdata_t*)Z_Malloc(sizeof(sfxdata_t), PU_STATIC, 0);
	sfx->sample_rate = (data[3] << 8) | data[2];
    sfx->num_samples = (data[7] << 24) | (data[6] << 16) | (data[5] << 8) | data[4];

    /* author note: forgive me for this transgression
       however, in my particular use case (fmod) i am required to keep the padding */
    // after header, first and last 16 bytes are padding
    //sfx->num_samples -= 32;
    sfx->data = ExpandSoundData(data + 8 /* + 16 */, sfx->num_samples);
    W_ReleaseLumpNum(lumpnum);
    
    sfxinfo->driver_data = sfx;
}

static int _DG_Snd_StartSound(sfxinfo_t* sfxinfo, int channel, int vol, int sep) {
    if (!sfxinfo) {
        return -1;
    }
    if (!sfxinfo->driver_data) {
        LoadSound(sfxinfo);
    }

    return DG_Snd_StartSound((sfxdata_t*)sfxinfo->driver_data, channel, vol, sep);
}

static void _DG_Snd_CacheSounds(sfxinfo_t* sounds, int num_sounds) {
    // i was hoping we'd have lumpnum by this point... but we do not
    // the "precaching" really does take place "pre"-everything
    /*
    sfxdata_t* sfx = (sfxdata_t*)Z_Malloc(num_sounds * sizeof(sfxdata_t), PU_STATIC, 0);
    for (int i = 0; i < num_sounds; i++) {
        LoadSound(&sounds[i]);
        sfx[i] = *(sfxdata_t*)sounds[i].driver_data;
    }
    DG_Snd_CacheSounds(sfx, num_sounds);
    Z_Free(sfx);
    */
}

static MUS_HANDLE _DG_Mus_RegisterSong(void* data, int len) {
    if (current_song.midi_data) {
        DG_Mus_StopSong();
		Z_Free(current_song.midi_data);
    }
    
    MEMFILE* fmus = mem_fopen_read(data, len);
    MEMFILE* fmid = mem_fopen_write();
    if (mus2mid(fmus, fmid)) {
		I_Error((char*)"mus2mid failed");
    }
    mem_fclose(fmus);
    void* mididata;
    size_t midilen;

    mem_get_buf(fmid, &mididata, &midilen);
    current_song.midi_data = (byte*)Z_Malloc(midilen, PU_STATIC, 0);
    current_song.len_bytes = midilen;
	memcpy(current_song.midi_data, mididata, midilen);
    return DG_Mus_RegisterSong(current_song.midi_data, midilen);
}

static void _DG_Mus_UnRegisterSong(MUS_HANDLE handle) {
	
    DG_Mus_UnRegisterSong(handle);

    if (current_song.midi_data) {
        Z_Free(current_song.midi_data);
	    current_song.midi_data = NULL;
	    current_song.len_bytes = 0;
    }
}

sound_module_t DG_sound_module = {
    sound_devices,
    arrlen(sound_devices),
    _DG_Snd_Init,
    DG_Snd_Shutdown,
    _DG_Snd_GetSfxLumpNum,
    DG_Snd_Update,
    DG_Snd_UpdateSoundParams,
    _DG_Snd_StartSound,
    DG_Snd_StopSound,
    DG_Snd_IsPlaying,
    _DG_Snd_CacheSounds,
};
music_module_t DG_music_module = {
    sound_devices,
    arrlen(sound_devices),
    DG_Mus_Init,
    DG_Mus_Shutdown,
    DG_Mus_SetVolume,
    DG_Mus_Pause,
    DG_Mus_Resume,
    _DG_Mus_RegisterSong,
    _DG_Mus_UnRegisterSong,
    DG_Mus_PlaySong,
    DG_Mus_StopSong,
    DG_Mus_IsPlaying,
    DG_Mus_Poll,
};
#endif
