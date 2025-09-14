using System.Numerics;
using NeodashObjectGenerator.Gen.Components;
using NeodashObjectGenerator.MCConverter;

namespace NeodashObjectGenerator.Minecraft.Blocks;

// Generates minecraft stairs
public class Stairs : Block
{
    public Vector3 Color { get; }
    // The direction the solid side faces
    public int Direction { get; }
    // Is regular or upside-down stairs
    public bool Lower { get; }
    // Whether the stair is an inner corner, outer corner, or regular stair.
    public bool? Inner { get; }
    // If inner is true or false, whether it is a left or right corner stair.
    public bool Left { get; }

    public Stairs(Vector3 color, int direction, bool lower, bool? inner, bool left)
    {
        Color = color;
        Direction = direction;
        Lower = lower;
        Inner = inner;
        Left = left;
    }

    public override IEnumerable<Component> Generate(SimpleWorld world, Location location)
    {
        var group = world.NextGroup();

        var flip = !Lower;
        var dc = world.Context;
        dc.VFlip = flip;

        // Generate a half slab
        var slab = world.MakePartialCube(location, dc.Use(Vector3.Zero), dc.Use(Pixel(16, 16, 8)), group);
        slab.AddBaseColor(Color);
        yield return slab;
        
        // Upper part of the stair
        var part = world.MakeFacing(Direction, context =>
        {
            context.VFlip = flip;
            context.YFlip = !Left;
            if (Inner is true or null)
            {
                return world.MakePartialCube(location, context.Use(Pixel(8, 0, 8)), context.Use(Pixel(16, 16, 16)), group);
            }
            return world.MakePartialCube(location, context.Use(Pixel(8, 0, 8)), context.Use(Pixel(16, 8, 16)), group);
        });
        part.AddBaseColor(Color);
        yield return part;

        // Extra bit for inner stairs
        if (Inner == true)
        {
            var corner = world.MakeFacing(Direction, context =>
            {
                context.VFlip = flip;
                context.YFlip = !Left;
                return world.MakePartialCube(location, context.Use(Pixel(0, 0, 8)), context.Use(Pixel(8, 8, 16)), group);
            });
            corner.AddBaseColor(Color);
            yield return corner;
        }
    }

    public static Func<string, TagCompound, Block> ForColor(Vector3 color)
    {
        return (s, tag) =>
        {
            var dir = Block.Direction((string) tag.Prop("facing"));
            var lower = (string) tag.Prop("half") == "bottom";
            var shape = (string) tag.Prop("shape");
            bool? inner = shape.Contains("inner") ? true : shape.Contains("outer") ? false : null;
            var left = shape.Contains("left");
            return new Stairs(color, dir, lower, inner, left) {Name = s};
        };
    }
}