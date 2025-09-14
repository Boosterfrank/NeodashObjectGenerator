using NeodashObjectGenerator.MCConverter;

namespace NeodashObjectGenerator.Minecraft;

public readonly record struct Coord(int X, int Z)
{
    public Coord(Coord c) : this(c.X, c.Z) { }

    public Coord Add(int dx, int dz) => new(X + dx, Z + dz);

    public Coord ChunkToAbsolute() => new(X * 16, Z * 16);

    public Coord AbsoluteToChunk() => new(X >> 4, Z >> 4);

    public Coord RegionToAbsolute() => new(X * 16 * 32, Z * 16 * 32);

    public Coord AbsoluteToRegion() => new(X >> 9, Z >> 9);

    public Coord RegionToChunk() => new(X * 32, Z * 32);

    public Coord ChunkToRegion()
    {
        return new Coord(X >> 5, Z >> 5);
    }

    public Location ToLocation(int y) => new(X, y, Z);

    //chunk coordinates within region, between (0, 0) and (31, 31)
    public Coord ChunkToRegionRelative()
    {
        return new Coord(X & 31, Z & 31);
    }
}