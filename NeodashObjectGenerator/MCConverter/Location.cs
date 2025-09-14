using System.Numerics;
using NeodashObjectGenerator.Minecraft;

namespace NeodashObjectGenerator.MCConverter;

// Represents a minecraft location
public readonly record struct Location(int X, int Y, int Z)
{
    public static Location One => new(1, 1, 1);

    public static Location South => new(0, 0, 1);
    
    public static Location East => new(1, 0, 0);
    
    public static Location Up => new(0, 1, 0);

    public static bool operator <(Location a, Location b)
    {
        return a.X < b.X && a.Y < b.Y && a.Z < b.Z;
    }
    
    public static bool operator >(Location a, Location b)
    {
        return a.X > b.X && a.Y > b.Y && a.Z > b.Z;
    }
    
    public static bool operator <=(Location a, Location b)
    {
        return a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z;
    }
    
    public static bool operator >=(Location a, Location b)
    {
        return a.X >= b.X && a.Y >= b.Y && a.Z >= b.Z;
    }

    public static Location operator +(Location a, Location b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    
    public static Location operator -(Location a, Location b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Location operator -(Location a) => new(-a.X, -a.Y, -a.Z);
    
    public static Location operator *(Location a, int i) => new(a.X * i, a.Y * i, a.Z * i);

    public Coord Coord() => new(X, Z);

    public Vector3 ToVector() => new(X, Y, Z);

    public Location Min(Location other) => new(Math.Min(X, other.X), Math.Min(Y, other.Y), Math.Min(Z, other.Z));

    public Location Max(Location other) => new(Math.Max(X, other.X), Math.Max(Y, other.Y), Math.Max(Z, other.Z));
}