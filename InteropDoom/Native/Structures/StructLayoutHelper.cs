namespace InteropDoom.Native.Structures;

internal unsafe readonly struct StructLayoutHelper
{
    // field offsets are 0-indexed
    // they are also not exactly "field" indices but more like "primitive component" indices
    public int[] Offsets { get; }
    public string Repr { get; }
    public int Alignment { get; }

    public StructLayoutHelper(string repr, int align = default)
    {
        if (align == default)
            align = sizeof(nint);
        
        Repr = repr;
        Alignment = align;
        Offsets = new int[repr.Length];
        
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
                _ => throw new InvalidOperationException("Invalid char in struct repr - allowed characters are: b (byte), s (short), i (int), l (long), p (nint)")
            };

            var misalignment = offset % size;
            if (misalignment > 0)
            {
                // align to size, e.g. "bibsl" => {byte, int, byte, short, long} => (b...iiiib.ss....llllllll)
                offset += size - misalignment;
            }
            Offsets[i] = offset;
            offset += size;
        }
    }
}
