using NeodashObjectGenerator.Minecraft;

namespace NeodashObjectGenerator.MCConverter;

// Represents a section, or sub-chunk.
public class Section
{
    // The region of blocks that this section corresponds to
    public Region Region { get; }
    // Y index of this section
    public sbyte YIndex { get; }

    // Block palette for this section
    public Tag[] Palette;
    
    // Block data for this section
    // Data is represented as palette indices, which are tightly packed into
    // longs using only the minimum number of bits required to index the entire palette.
    // Minimum of 4 bits are used. Bits for an index will not cross from one long to another,
    // so there may be some empty space at the end of each long depending on the number
    // of bits used.
    private long[] _data;
    // Bits per index
    private int _entryBits;
    // Number of indices per long
    private int _entriesPerElement;

    public Section(Chunk chunk, TagCompound data)
    {
        YIndex = (sbyte) data["Y"];
        var min = chunk.Coords.ChunkToAbsolute().ToLocation(YIndex * 16);
        Region = new Region(min, min + new Location(15, 15, 15));

        var states = (Dictionary<string, Tag>) data["block_states"];
        Palette = (Tag[]) states["palette"];
        if (states.TryGetValue("data", out var chunkData))
        {
            _data = (long[]) chunkData;
        }

        _entryBits = Math.Max(4, Log2(Palette.Length));
        _entriesPerElement = sizeof(long) * 8 / _entryBits;
    }

    private int Log2(int n)
    {
        var bits = 0;
        if (n > 0xffff) {
            n >>= 16;
            bits = 0x10;
        }
        if (n > 0xff) {
            n >>= 8;
            bits |= 0x8;
        }
        if (n > 0xf) {
            n >>= 4;
            bits |= 0x4;
        }
        if (n > 0x3) {
            n >>= 2;
            bits |= 0x2;
        }
        if (n > 0x1) {
            bits |= 0x1;
        }
        return bits;
    }

    // Get the index of a block in the data.
    // This is in chunk-coordinates: [0..16]
    private int DataIndex(int x, int y, int z) => y * 16 * 16 + z * 16 + x;

    // Lookup the index in the palette for a particular block
    public int PaletteIndex(int x, int y, int z)
    {
        // If the entire section is taken up by a single block, there will be no data array
        if (_data == null) return 0;
        var index = DataIndex(x, y, z);
        return (int) ((_data[index / _entriesPerElement] >> (index % _entriesPerElement * _entryBits)) & ((1 << _entryBits) - 1));
    }

    // Lookup the index in the palette for a particular block
    public int PaletteIndex(Location loc) => PaletteIndex(loc.X, loc.Y, loc.Z);

    // Enumerate every block in this section, optionally specifying a smaller region of blocks to get.
    // Only the blocks contained in both the section and the given region are returned.
    public IEnumerable<(Location Loc, TagCompound Block)> AllBlocks(Region region = null)
    {
        var overlap = region?.Overlap(Region) ?? Region;
        var min = Region.Min;
        foreach (var location in overlap.Locations())
        {
            yield return (location, (TagCompound) Palette[PaletteIndex(location - min)]);
        }
    }
}