using System.Numerics;

namespace NeodashObjectGenerator.Gen;

// Color conversion methods
public static class Color
{
    public static Vector3 White => Vector3.One;
    public static Vector3 Black => Vector3.Zero;
    
    public static Vector3 Rgb(int r, int g, int b)
    {
        return new Vector3(r / 255f, g / 255f, b / 255f);
    }
}