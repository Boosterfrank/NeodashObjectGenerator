using System.Numerics;
using NeodashObjectGenerator.Gen;
using NeodashObjectGenerator.Gen.Components;
using NeodashObjectGenerator.MCConverter;

namespace NeodashObjectGenerator.Minecraft.Blocks;

// Block where the top section is a different color than the rest of the block, e.g. grass
public class ToppedBlock : Block, ISolid
{
    public Vector3 TopColor;
    public Vector3 BaseColor;

    public bool Path;

    public ToppedBlock(Vector3 topColor, Vector3 baseColor)
    {
        TopColor = topColor;
        BaseColor = baseColor;
    }

    public override IEnumerable<Component> Generate(SimpleWorld world, Location location)
    {
        var group = world.NextGroup();
        var top = world.MakePartialCube(location, Pixel(0, 0, 13), Path ? Pixel(16, 16, 15) : Vector3.One, group);
        top.AddBaseColor(TopColor);
        yield return top;
        
        var bottom = world.MakePartialCube(location, Vector3.Zero, Pixel(16, 16, 13), group);
        bottom.AddBaseColor(BaseColor);
        yield return bottom;
    }

    public static Func<string, TagCompound, Block> Colors(Vector3 top, Vector3 bottom, bool path = false)
    {
        return (s, tag) =>
        {
            // Change the top color to white if the "snowy" tag is true
            if (tag.TryGetProp("snowy", out var snowTag) && (string) snowTag == "true")
            {
                top = Color.White;
            }
            return new ToppedBlock(top, bottom) {Name = s, Path = path};
        };
    }
}