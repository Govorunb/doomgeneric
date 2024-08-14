namespace InteropDoom.Native.Structures;

internal unsafe readonly struct StructLayoutHelper
{
    public List<int> Offsets { get; }
    public string Repr { get; }
    public int Alignment { get; }

    public StructLayoutHelper(string repr, int align = default)
    {
        if (align == default)
            align = sizeof(nint);
        
        Repr = repr;
        Alignment = align;
        Offsets = new(repr.Length);
        
        int offset = 0;
        for (int i = 0; i < repr.Length; i++)
        {
            var size = repr[i] switch
            {
                'b' => sizeof(byte),
                's' => sizeof(short),
                'i' => sizeof(int),
                'l' => sizeof(long),
                'p' => sizeof(nint),
                '_' => 0, // ignored
                _ => throw new InvalidOperationException("Invalid char in struct repr - allowed characters are: b (byte), s (short), i (int), l (long), p (nint), _ (ignored)")
            };
            if (size == 0) continue;
            var misalignment = offset % size;
            if (misalignment > 0)
            {
                // align to size (e.g. {byte, int, byte, short, long} => (b...iiiib.ss....llllllll)
                offset += size - misalignment;
            }
            Offsets.Add(offset);
            offset += size;
        }
    }
}
