using System.Numerics;
using NeodashObjectGenerator.Gen;
using NeodashObjectGenerator.Gen.Components;
using NeodashObjectGenerator.MCConverter;

namespace NeodashObjectGenerator.Minecraft.Blocks;

// Generate tall grass using spikes
// Also used for double tall grass
public class TallGrass : Block
{
    public bool DoubleTall { get; }
    
    public TallGrass() : this(false) { }

    public TallGrass(bool doubleTall)
    {
        DoubleTall = doubleTall;
    }

    public override IEnumerable<Component> Generate(SimpleWorld world, Location location)
    {
        var block = world.BlockToWorld(location);
        var group = world.NextGroup();

        if (DoubleTall)
        {
            var spikes0 = new Spikes(block + world.Shift(-1f, 0f, 0.19249697f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 11.888429f), group);
            spikes0.AddGlowIntensity(0.15f);
            spikes0.AddGlowColor(new Vector3(0f, 1f, 0f));
            spikes0.AddBaseColor(new Vector3(0.120429f, 0.445003f, 0.186865f));
            yield return spikes0;
        }
        else
        {
            var spikes0 = new Spikes(block + world.Shift(0f, 0f, -0.20000002f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 5f), group);
            spikes0.AddGlowIntensity(0.15f);
            spikes0.AddBaseColor(new Vector3(0.120429f, 0.445003f, 0.186865f));
            spikes0.AddGlowColor(new Vector3(0f, 1f, 0f));
            yield return spikes0;
        }
    }

    public static Block Read(string name, TagCompound tag)
    {
        var isLower = (string) tag.Prop("half") == "lower";
        return !isLower ? null : new TallGrass(true);
    }
}