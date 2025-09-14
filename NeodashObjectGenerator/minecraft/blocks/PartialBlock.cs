using System.Numerics;
using NeodashObjectGenerator.Gen.Components;
using NeodashObjectGenerator.MCConverter;

namespace NeodashObjectGenerator.Minecraft.Blocks;

// Represents a block that only takes up a portion of a full block, e.g. snow
public class PartialBlock : FlatColorBlock
{
    public Vector3 Lower;
    public Vector3 Upper;

    public PartialBlock(Vector3 color, Vector3 lower, Vector3 upper) : base(color)
    {
        Lower = lower.Min(upper);
        Upper = lower.Max(upper);
    }

    public override IEnumerable<Component> Generate(SimpleWorld world, Location location)
    {
        var cube = world.MakePartialCube(location, Lower, Upper);
        cube.AddBaseColor(Color);
        yield return cube;
    }
}