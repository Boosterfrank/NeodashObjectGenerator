using System.Numerics;
using NeodashObjectGenerator.Gen;

namespace NeodashObjectGenerator.MCConverter;

// Represents a collection of blocks stored at given locations
public class SimpleWorld
{
    public Dictionary<Location, Block> Blocks;

    public Vector3 WorldScale = Vector3.One;

    public Vector3 Origin;
    public Vector3 Offset;

    public string Header;

    public readonly DirectionContext Context = new();

    public SimpleWorld()
    {
        Blocks = new Dictionary<Location, Block>();
    }

    public SimpleWorld(Region region)
    {
        Blocks = new Dictionary<Location, Block>((int) region.Volume);
    }

    public int NextGroup() => 0;

    // Sets the offset so that a given block location ends up at a given neodash location (after scaling)
    // All other blocks will be scaled around this location
    public void SetupOffset(Location block, Vector3 position)
    {
        Origin = position;
        Offset = position - block.McToNeodash() * Sizes.BlockSize;
    }

    // Get the neodash location from a block location, using the defined world origin setup.
    public Vector3 BlockToWorld(Location block)
    {
        return (block.McToNeodash() * Sizes.BlockSize + Offset - Origin) * WorldScale + Origin;
    }

    // Set a block at a particular location
    public Block this[Location location]
    {
        get => Blocks.TryGetValue(location, out var block) ? block : null;
        set
        {
            if (value is null) Blocks.Remove(location);
            else Blocks[location] = value;
        }
    }

    // Perform an action on a given block, but only if the location being checked is greater
    // than the current location. This is so that checks that happen both ways will only occur once.
    public void CheckBidirectional(Location from, Location to, Action<Block> action)
    {
        if (!(from <= to)) return;
        Check(to, action);
    }

    // Perform an action on a given block.
    public void Check(Location to, Action<Block> action)
    {
        Blocks.TryGetValue(to, out var block);
        action(block);
    }

    // Allows you to perform an action in both the north/south and east/west directions
    // The action should use the DirectionContext for any directional offsets, which will
    // allow the action to automatically apply in both directions
    public void MultiDirectionAction(Action<DirectionContext> action)
    {
        Context.Reset();
        Context.Flip = false;
        action(Context);
        Context.Flip = true;
        action(Context);
    }
    
    // Generate the components from each block and write them to a file.
    public void GenerateFile(string path, bool append = false)
    {
        using var output = new StreamWriter(path, append);
        if (!string.IsNullOrWhiteSpace(Header)) output.WriteLine(Header);
        foreach (var (location, block) in Blocks)
        {
            foreach (var component in block.Generate(this, location))
            {
                if (component == null) continue;
                output.WriteLine(component.ToString());
            }
        }
    }
}