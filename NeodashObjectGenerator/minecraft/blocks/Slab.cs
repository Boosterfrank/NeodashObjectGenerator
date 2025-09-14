using System.Numerics;
using NeodashObjectGenerator.Gen.Components;
using NeodashObjectGenerator.MCConverter;

namespace NeodashObjectGenerator.Minecraft.Blocks;

// Represents a half slab
// Can represent a lower, upper, or double half slab.
public class Slab : Block
{
    public Vector3 Color { get; }
    
    public bool Lower { get; }
    public bool Upper { get; }

    public Slab(Vector3 color, bool? lower)
    {
        Color = color;
        if (lower == true) Lower = true;
        else if (lower == false) Upper = true;
        else Lower = Upper = true;
    }

    private Component GenerateSlab(int start, int end, SimpleWorld world, Location location)
    {
        var slab = world.MakePartialCube(location, Pixel(0, 0, start), Pixel(16, 16, end));
        slab.AddBaseColor(Color);
        return slab;
    }

    public override IEnumerable<Component> Generate(SimpleWorld world, Location location)
    {
        if (Lower) yield return GenerateSlab(0, 8, world, location);
        if (Upper) yield return GenerateSlab(8, 16, world, location);
    }

    public static Func<string, TagCompound, Block> ForColor(Vector3 color)
    {
        return (s, tag) =>
        {
            var type = (string) tag.Prop("type");
            bool? lower = type switch
            {
                "bottom" => true,
                "top" => false,
                _ => null,
            };
            return new Slab(color, lower) {Name = s};
        };
    }
}