using System.Reflection;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using NAudio.Midi;
using DWMMidiEvent = Melanchall.DryWetMidi.Core.MidiEvent;
using NMidiEvent = NAudio.Midi.MidiEvent;

namespace AvaloniaPlayer.Doom.Audio;
internal class NAudioOutputDevice(int device) : MidiOut(device), IOutputDevice
{
    private int _volume;
    /// <summary>
    /// Volume in range 0-127.
    /// </summary>
    public int MidiVolume
    {
        get => _volume;
        set
        {
            _volume = value;
            UpdateVolume();
        }
    }
    [Obsolete($"On Windows, midiOutSetVolume sets the application's volume (in the Volume Mixer). Use {nameof(MidiVolume)} instead.", error: true)]
    public new float Volume { get; set; }

    public event EventHandler<MidiEventSentEventArgs>? EventSent;
    public void PrepareForEventsSending()
    {
        Reset();
    }

    public void SendEvent(DWMMidiEvent midiEvent)
    {
        if (midiEvent is Melanchall.DryWetMidi.Core.ControlChangeEvent cc)
        {
            cc.ControlValue = (SevenBitNumber)(cc.ControlValue * MidiVolume / 127f);
        }
        Send(midiEvent.ToMessage());
    }

    private void UpdateVolume()
    {
        for (byte channel = 1; channel <= 16; channel++)
        {
            var msg = new NAudio.Midi.ControlChangeEvent(0, channel, MidiController.MainVolume, MidiVolume);
            Send(msg.GetAsShortMessage());
        }
    }

    public void Update()
    {
        UpdateVolume();
    }
}

internal static class DwmMidiEventExtensions
{
    private static readonly MidiEventToBytesConverter _conv = new(3);
    private static readonly MethodInfo _channelGetStatusByte = Type.GetType("Melanchall.DryWetMidi.Core.ChannelEventWriter, Melanchall.DryWetMidi")!.GetMethod("GetStatusByte")!;
    private static readonly MethodInfo _systemRealTimeGetStatusByte = Type.GetType("Melanchall.DryWetMidi.Core.SystemRealTimeEventWriter, Melanchall.DryWetMidi")!.GetMethod("GetStatusByte")!;

    private static readonly object _channelEventWriterInstance = typeof(OutputDevice)!.GetField("ChannelEventWriter", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
    private static readonly object _systemRealTimeEventWriterInstance = typeof(OutputDevice)!.GetField("SystemRealTimeEventWriter", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;

    public static int ToMessage(this DWMMidiEvent midiEvent)
    {
        switch (midiEvent)
        {
            case ChannelEvent channelEvent:
                return (byte)_channelGetStatusByte.Invoke(_channelEventWriterInstance, [channelEvent])! | channelEvent.GetDataBytes();
            case SystemRealTimeEvent midiEvent2:
                return (byte)_systemRealTimeGetStatusByte.Invoke(_systemRealTimeEventWriterInstance, [midiEvent2])!;
            default:
            {
                var array = _conv.Convert(midiEvent, 3);
                return array[0] | (array[1] << 8) | (array[2] << 16);
            }
        }
    }
    public static NMidiEvent ToNAudio(this DWMMidiEvent evt)
    {
        return new(evt.DeltaTime, 1, evt.EventType.ToNAudio());
    }

    public static MidiCommandCode ToNAudio(this MidiEventType type)
    {
        return type switch
        {
            MidiEventType.ProgramChange => MidiCommandCode.PatchChange,
            MidiEventType.PitchBend => MidiCommandCode.PitchWheelChange,
            _ => Enum.Parse<MidiCommandCode>(type.ToString())
        };
    }
    private static readonly FieldInfo _dataByte1Field = typeof(ChannelEvent).GetField("_dataByte1", BindingFlags.NonPublic | BindingFlags.Instance)!;
    private static readonly FieldInfo _dataByte2Field = typeof(ChannelEvent).GetField("_dataByte2", BindingFlags.NonPublic | BindingFlags.Instance)!;
    public static byte GetDataByte1(this DWMMidiEvent evt)
        => (byte)_dataByte1Field.GetValue(evt)!;
    public static byte GetDataByte2(this DWMMidiEvent evt)
        => (byte)_dataByte2Field.GetValue(evt)!;
    public static int GetDataBytes(this DWMMidiEvent evt)
        => evt.GetDataByte1() << 8 | evt.GetDataByte2() << 16;
}
