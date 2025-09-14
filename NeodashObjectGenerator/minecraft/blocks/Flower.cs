using System.Numerics;
using NeodashObjectGenerator.Gen.Components;
using NeodashObjectGenerator.MCConverter;

namespace NeodashObjectGenerator.Minecraft.Blocks;

// Generate a flower using a peg and a saw
public class Flower : Block
{
    public Vector3 Color { get; }
    
    public Flower(Vector3 color)
    {
        Color = color;
    }
    
    public override IEnumerable<Component> Generate(SimpleWorld world, Location location)
    {
        var block = world.BlockToWorld(location);
        var group = world.NextGroup();

        var peg10 = new Peg(block + world.Shift(0f, 0f, -0.5f), new Vector3(0f, 0f, 0f), new Vector3(0.423f, 0.423f, 1.06f), group);
        peg10.AddBaseColor(Color);
        yield return peg10;

        var sawTrap10 = new Saw(block, new Vector3(90f, 0f, 0f), new Vector3(0.2f, 0.6f, 0.2f), group);
        yield return sawTrap10;

    }
}