namespace NeodashObjectGenerator.MCConverter;

// Represents a space within two location.
public class Region
{
    public Location Min { get; }
    public Location Max { get; }

    // Create an empty region with no volume
    public Region() => Min = new Location(1, 1, 1);

    // Create a region from two locations.
    // The region includes both locations.
    public Region(Location a, Location b)
    {
        Min = a.Min(b);
        Max = a.Max(b);
    }
    
    public long Volume
    {
        get
        {
            var vol = Max - Min + new Location(1, 1, 1);
            return vol.X * vol.Y * vol.Z;
        }
    }

    // Whether the region is empty (volume = 0)
    public bool Empty => !(Min <= Max);

    public bool Contains(Location loc) => loc >= Min && loc <= Max;

    // Get the overlap (intersection) of two regions, this will return an empty region
    // if there is no overlap.
    public Region Overlap(Region other)
    {
        if (Empty || other.Empty) return new Region();
        var midMin = Min.Max(other.Min);
        var midMax = Max.Min(other.Max);
        return !(midMin <= midMax) ? new Region() : new Region(midMin, midMax);
    }

    // Get every location contained in the region.
    public IEnumerable<Location> Locations()
    {
        if (Empty) yield break;
        for (int y = Min.Y, my = Max.Y; y <= my; y++)
        {
            for (int z = Min.Z, mz = Max.Z; z <= mz; z++)
            {
                for (int x = Min.X, mx = Max.X; x <= mx; x++)
                {
                    yield return new Location(x, y, z);
                }
            }
        }
    }

    protected bool Equals(Region other) => Min.Equals(other.Min) && Max.Equals(other.Max);

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Region) obj);
    }

    public override int GetHashCode() => HashCode.Combine(Min, Max);

    public override string ToString() => $"{{Min={Min}, Max={Max}}}";
}