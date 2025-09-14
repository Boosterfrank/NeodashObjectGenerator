using System.Numerics;
using NeodashObjectGenerator.Gen;
using NeodashObjectGenerator.Gen.Components;
using NeodashObjectGenerator.MCConverter;

namespace NeodashObjectGenerator.Minecraft.Blocks;

// Generates a mostly solid block with a specified color
public class FlatColorBlock : Block, ISolid
{
    public Vector3 Color { get; }

    // For blocks that are slightly less than a full block tall
    public int PixelHeight { get; init; } = 16;
    
    // If the entire cube should glow instead of be a solid color
    public bool Glowing { get; init; }

    public FlatColorBlock(Vector3 color)
    {
        Color = color;
    }

    public override IEnumerable<Component> Generate(SimpleWorld world, Location location)
    {
        var cube = world.MakePartialCube(location, Vector3.Zero, Pixel(16, 16, PixelHeight));
        if (Glowing)
        {
            cube.AddBandThickness(0.5f);
            cube.AddGlowColor(Color);
        }
        else cube.AddBaseColor(Color);
        yield return cube;
    }

    public static Func<string, TagCompound, Block> ForColor(Vector3 color, int height = 16, bool glowing = false)
    {
        return (s, _) => new FlatColorBlock(color) {Name = s, PixelHeight = height, Glowing = glowing};
    }
}