using NeodashObjectGenerator.Minecraft;

namespace NeodashObjectGenerator.MCConverter;

// Converts NBT tags into block objects
public class BlockConverter
{
    public Dictionary<string, Func<string, TagCompound, Block>> Converters;

    public BlockConverter()
    {
        Converters = new Dictionary<string, Func<string, TagCompound, Block>>();
    }

    // Add converter
    public void Add(string name, Func<string, TagCompound, Block> converter)
    {
        Converters[name] = converter;
    }

    // Ignore the specified block type, i.e. return null.
    public void Ignore(string name)
    {
        Add(name, null);
    }

    // Alias a block type to another block type.
    // When requested to convert the given block, it's name will
    // be changed to a different block and sent back to the converter.
    public void Alias(string name, string copy)
    {
        Add(name, (_, t) =>
        {
            ((TagString) t["Name"]).ChangePayload(copy);
            return Convert(t);
        });
    }

    // Get a block that doesn't require an NBT tag.
    public Block Get(string name)
    {
        return Converters[name](name, null);
    }

    // Convert an NBT tag into a block.
    public Block Convert(TagCompound tag)
    {
        var name = (string) tag["Name"];
        if (!Converters.TryGetValue(name, out var converter))
        {
            throw new Exception($"There is no converter for type \"{name}\"");
        }
        return converter?.Invoke(name, tag);
    }
}