using System.Numerics;
using NeodashObjectGenerator.MCConverter;

namespace NeodashObjectGenerator.Minecraft.Blocks;

// Generates snow
public class Snow : PartialBlock
{
    public Snow(int layers) : base(Vector3.One, Vector3.Zero, new Vector3(1, 1, layers / 8f)) { }

    public static Block Read(string s, TagCompound tag)
    {
        return new Snow((int) tag.Prop("layers"));
    }
}