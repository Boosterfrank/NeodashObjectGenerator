using NeodashObjectGenerator.Minecraft;

namespace NeodashObjectGenerator.MCConverter;

// Provides functionality to read the data from a minecraft world.
public class WorldReader
{
    private string _regionFolder;
    
    // Currently loaded region
    private RegionFile _currentRegion;

    // Currently used chunk
    private Chunk _currentChunk;

    public WorldReader(string regionFolder)
    {
        _regionFolder = regionFolder;
    }

    // Load the region at the given region coordinates
    public void LoadRegion(int x, int z)
    {
        if (_currentRegion?.Coords.X == x && _currentRegion?.Coords.Z == z) return;

        var path = Path.Combine(_regionFolder, $"r.{x}.{z}.mca");
        if (!File.Exists(path)) throw new ArgumentException($"Region file {x}, {z} does not exist.");
        
        _currentRegion = new RegionFile(path);
    }

    // Load the chunk at the given chunk coordinates.
    // This will also load the required region if necessary
    public void LoadChunk(int x, int z)
    {
        if (_currentChunk?.Coords.X == x && _currentChunk?.Coords.Z == z) return;
        
        var chunkCoord = new Coord(x, z);
        var regionCoord = chunkCoord.ChunkToRegion();
        LoadRegion(regionCoord.X, regionCoord.Z);
        chunkCoord = chunkCoord.ChunkToRegionRelative();
        _currentChunk = _currentRegion!.Chunks[chunkCoord.X, chunkCoord.Z];
    }

    // Get all sections in a chunk.
    // Remove extra means sections outside the 0-256 block range are removed.
    public IEnumerable<Section> ChunkSections(bool removeExtra = true)
    {
        if (_currentChunk == null) yield break;
        foreach (var tag in (Tag[]) _currentChunk.Root["sections"])
        {
            if (removeExtra && (sbyte) tag["Y"] is < 0 or > 15) continue;
            yield return new Section(_currentChunk, (TagCompound) tag);
        }
    }

    // Get the collection of chunks overlapped by the given region
    public IEnumerable<Coord> GetRequiredChunks(Region region)
    {
        var minChunk = region.Min.Coord().AbsoluteToChunk();
        var maxChunk = region.Max.Coord().AbsoluteToChunk();
        for (var x = minChunk.X; x <= maxChunk.X; x++)
        {
            for (var z = minChunk.Z; z <= maxChunk.Z; z++)
            {
                yield return new Coord(x, z);
            }
        }
    }

    // Read all blocks in a given region
    public IEnumerable<(Location Loc, TagCompound Block)> ReadAllBlocks(Region region)
    {
        // Required chunks are grouped by region so that each region file only needs to
        // be loaded once.
        var regions = GetRequiredChunks(region).GroupBy(c => c.ChunkToRegion());
        foreach (var requiredRegion in regions)
        {
            foreach (var chunk in requiredRegion)
            {
                LoadChunk(chunk.X, chunk.Z);
                foreach (var section in ChunkSections())
                {
                    foreach (var blockInfo in section.AllBlocks(region))
                    {
                        yield return blockInfo;
                    }
                }
            }
        }
    }
}