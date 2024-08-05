using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using NAudio.Midi;
using MidiEvent = Melanchall.DryWetMidi.Core.MidiEvent;
using NControlChangeEvent = NAudio.Midi.ControlChangeEvent;
using DWMControlChangeEvent = Melanchall.DryWetMidi.Core.ControlChangeEvent;

namespace AvaloniaPlayer.Doom.Audio;
internal class NAudioOutputDevice(int device) : MidiOut(device), IOutputDevice
{
    private float _volume;
    /// <summary>
    /// Volume in range 0..1.
    /// </summary>
    public float MidiVolume
    {
        get => _volume;
        set
        {
            _volume = value;
            UpdateVolume();
        }
    }
    [Obsolete($"On Windows, midiOutSetVolume sets the whole application's volume (in the Volume Mixer). Use {nameof(MidiVolume)} instead.", error: true)]
    public new float Volume { get; set; }

    public event EventHandler<MidiEventSentEventArgs>? EventSent;
    public void PrepareForEventsSending()
    {
        Reset();
    }

    private static readonly MidiEventToBytesConverter _conv = new(4);
    public void SendEvent(MidiEvent midiEvent)
    {
        if (midiEvent is DWMControlChangeEvent cc)
        {
            cc.ControlValue = (SevenBitNumber)(cc.ControlValue * MidiVolume);
        }

        var bytes = _conv.Convert(midiEvent, 4);
        var message = BitConverter.ToInt32(bytes.AsSpan());
        Send(message);
    }

    private void UpdateVolume()
    {
        for (var channel = 1; channel <= 16; channel++)
        {
            var msg = new NControlChangeEvent(0, channel, MidiController.MainVolume, (int)(MidiVolume * 127));
            Send(msg.GetAsShortMessage());
        }
    }

    public void Update()
    {
        UpdateVolume();
    }
}