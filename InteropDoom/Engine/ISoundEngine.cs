namespace InteropDoom.Engine;

/// <summary>
/// Represents the part of the engine responsible for playing sound effects.<br/>
/// <br/>
/// Sound effect data is 16-bit signed PCM, generally in 11025Hz.
/// </summary>
public interface ISoundEngine
{
    /// <summary>
    /// Initialize the engine. Should be idempotent (multiple inits are allowed and have no effect beyond the first).
    /// </summary>
    /// <returns>Whether initialization was successful.</returns>
    bool Init();
    /// <summary>
    /// Shut down the engine if it's running and release any resources.
    /// </summary>
    void Shutdown();
    /// <summary>
    /// Start playing the given sound effect.
    /// </summary>
    /// <param name="sfxInfo"></param>
    /// <param name="channel">
    /// Sound channel index. Used by the game to limit the number of concurrently playing sounds.<br/>
    /// <br/>
    /// Note to implementers: You can ignore this parameter to use your own engine-specific limits.
    /// </param>
    /// <param name="volume">Volume, between 0 and 1.</param>
    /// <param name="pan">Stereo separation, between -1 (left) and 1 (right).</param>
    /// <returns>Arbitrary (native integer sized) handle to identify the playing sound effect.</returns>
    nint Start(SfxInfo sfxInfo, int channel, float volume, float pan);
    /// <summary>
    /// Stop a currently playing sound effect.
    /// </summary>
    /// <param name="handle">Handle returned by <see cref="Start"/>.</param>
    void Stop(nint handle);
    /// <param name="handle"><inheritdoc cref="Stop"/></param>
    bool IsPlaying(nint handle);
    /// <summary>Update the engine. Called by the game.</summary>
    void Update();
    /// <summary>
    /// Update sound parameters for a playing sound effect.
    /// </summary>
    /// <param name="handle"><inheritdoc cref="Stop"/></param>
    /// <param name="volume"><inheritdoc cref="Start"/></param>
    /// <param name="pan"><inheritdoc cref="Start"/></param>
    void UpdateSoundParams(nint handle, float volume, float pan);
}
