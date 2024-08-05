namespace InteropDoom.Engine;

public interface IMusicEngine
{
    /// <inheritdoc cref="ISoundEngine.Init"/>
    bool Init();
    /// <inheritdoc cref="ISoundEngine.Shutdown"/>
    void Shutdown();

    /// <summary>
    /// Register song data with the music engine.
    /// </summary>
    /// <remarks>
    /// The <paramref name="midiStream"/> is <b>not</b> guaranteed to live until <see cref="UnRegisterSong(nint)"/> is called.<br/>
    /// You should read the whole stream and parse it into a MIDI file representation.
    /// </remarks>
    /// <param name="midiStream">A read-only <see cref="Stream"/> of MIDI data.</param>
    /// <returns>
    /// An arbitrary (native integer sized) handle to identify the song - e.g. an array index like 5, or a pointer.
    /// </returns>
    nint RegisterSong(Stream midiStream);
    /// <summary>
    /// Unregister a previously registered song.
    /// </summary>
    /// <param name="handle">Song handle returned by <see cref="RegisterSong(Stream)"/></param>
    void UnRegisterSong(nint handle);

    /// <summary>
    /// Play a previously registered song.
    /// </summary>
    /// <param name="handle">The handle (see <see cref="RegisterSong(Stream)"/>) of the song to play.</param>
    /// <param name="looping">Whether the song should loop.</param>
    void Play(nint handle, bool looping);
    void Stop();
    /// <inheritdoc cref="ISoundEngine.Update"/>
    void Update();
    bool IsPlaying();
    void Pause();
    void Resume();
    /// <param name="volume">Volume value, from 0 to 1.</param>
    void SetVolume(float volume);
}
