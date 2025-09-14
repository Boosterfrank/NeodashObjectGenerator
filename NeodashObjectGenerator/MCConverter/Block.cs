using System.Numerics;
using NeodashObjectGenerator.Gen.Components;
using NeodashObjectGenerator.Minecraft;

namespace NeodashObjectGenerator.MCConverter;

// Base class for objects that can be generated in the world
public abstract class Block
{
    // Name for the object that gets inserted in the CSV
    public string Name { get; init; }

    // Get the collection of components (Neodash objects) that are generated
    // to make up this block.
    // World is provided for blocks that need context, such as surrounding blocks.
    // Location is where to generate the block.
    // The world object provides a method to convert from block location to world location.
    public abstract IEnumerable<Component> Generate(SimpleWorld world, Location location);

    // Quickly get the block coordinates for a sub-pixel position.
    // Pixel(8, 8, 8) represents the middle of the block
    public static Vector3 Pixel(float x, float y, float z)
    {
        return new Vector3(x / 16f, y / 16f, z / 16f);
    }

    // Get the facing direction from the facing string
    public static int Direction(string facing)
    {
        return facing switch
        {
            "north" => 3,
            "east" => 0,
            "south" => 1,
            "west" => 2,
            _ => 0,
        };
    }

    // Quickly create a block tag for testing purposes.
    // name is the name of the block.
    // props is a comma separated list of name=value pairs for the block properties.
    public static TagCompound MakeTag(string name, string props = null)
    {
        var properties = new Dictionary<string, Tag>();
        if (props != null)
        {
            foreach (var prop in props.Split(','))
            {
                var parts = prop.Split('=');
                properties[parts[0]] = new TagString(parts[1], parts[0]);
            }
        }
        var nameTag = new TagString(name, "Name");

        var block = new Dictionary<string, Tag>
        {
            ["Name"] = nameTag,
            ["Properties"] = new TagCompound(properties, "Properties")
        };
        return new TagCompound(block);
    }
}