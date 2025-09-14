using System.Numerics;
using NeodashObjectGenerator.Gen.Components;
using NeodashObjectGenerator.MCConverter;

namespace NeodashObjectGenerator.Minecraft.Blocks;

// Generate a minecraft fence
public class Fence : Block
{
    public Vector3 Color { get; }

    public Fence(Vector3 color)
    {
        Color = color;
    }

    // Generate the upper and lower fence connectors
    private void GenerateFenceConnectors(SimpleWorld world, Location location, DirectionContext context, List<Component> result, int group)
    {
        var lower = world.MakePartialCube(location, context.Use(Pixel(7, 10, 6)), context.Use(Pixel(9, 22, 9)), group);
        lower.AddBaseColor(Color);
        result.Add(lower);
            
        var upper = world.MakePartialCube(location, context.Use(Pixel(7, 10, 12)), context.Use(Pixel(9, 22, 15)), group);
        upper.AddBaseColor(Color);
        result.Add(upper);
    }

    public override IEnumerable<Component> Generate(SimpleWorld world, Location location)
    {
        var group = world.NextGroup();

        var post = world.MakePartialCube(location, Pixel(6, 6, 0), Pixel(10, 10, 16), group);
        post.AddBaseColor(Color);
        yield return post;

        var parts = new List<Component>(4);
        // Connect to other fences and solid blocks
        world.MultiDirectionAction(context =>
        {
            // Check for adjacent fences or solid blocks
            world.CheckBidirectional(location, location + context.Use(Location.South), other =>
            {
                if ((other is not Fence f || Color != f.Color) && other is not ISolid) return;
                GenerateFenceConnectors(world, location, context, parts, group);
            });
            // Special check: solid blocks that are behind the fence
            world.Check(location - context.Use(Location.South), other =>
            {
                if (other is not ISolid) return;
                GenerateFenceConnectors(world, location - context.Use(Location.South), context, parts, group);
            });
        });

        foreach (var component in parts)
        {
            yield return component;
        }
    }
}