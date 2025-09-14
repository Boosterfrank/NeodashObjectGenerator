using System.Numerics;

namespace NeodashObjectGenerator.MCConverter;

// Used for performing actions in a direction-independent manner
// When Flip is set to true, used locations will be north-east transposed.
// When clockwise is a non-zero value, the location will be rotated.
// Vector only:
// VFLip flips a position across Z = 0.5
// YFlip flips a position across Y = 0.5
public class DirectionContext
{
    private int _clockwise;
    
    public bool Flip { get; set; }
    public bool VFlip { get; set; }
    public bool YFlip { get; set; }

    public int Clockwise
    {
        get => _clockwise;
        set => _clockwise = (value % 4 + 4) % 4;
    }

    public void Reset()
    {
        Clockwise = 0;
        Flip = false;
        VFlip = false;
        YFlip = false;
    }

    // Neodash locations are XY transposed
    public Vector3 Use(Vector3 v)
    {
        if (Flip) v = v with {X = v.Y, Y = v.X};
        if (VFlip) v = v with {Z = 1 - v.Z};
        if (YFlip) v = v with {Y = 1 - v.Y};
        if (_clockwise != 0)
        {
            for (var i = 0; i < _clockwise; i++)
            {
                v = v.Clockwise(new Vector3(0.5f, 0.5f, 0.5f));
            }
        }
        return v;
    }

    // Minecraft locations are XZ transposed
    public Location Use(Location l)
    {
        if (Flip) l = l with {X = l.Z, Z = l.X};
        if (_clockwise != 0)
        {
            for (var i = 0; i < _clockwise; i++)
            {
                l = l.Clockwise();
            }
        }
        return l;
    }
}