using System.Numerics;
using NeodashObjectGenerator.Gen;
using NeodashObjectGenerator.Gen.Components;
using NeodashObjectGenerator.Minecraft;
using NeodashObjectGenerator.Minecraft.Blocks;

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
        Blocks = new Dictionary<Location, Block>((int)region.Volume);
    }

    public int NextGroup() => 0;

    public void SetupOffset(Location block, Vector3 position)
    {
        Origin = position;
        Offset = position - block.McToNeodash() * Sizes.BlockSize;
    }

    public Vector3 BlockToWorld(Location block)
    {
        return (block.McToNeodash() * Sizes.BlockSize + Offset - Origin) * WorldScale + Origin;
    }

    public Block this[Location location]
    {
        get => Blocks.TryGetValue(location, out var block) ? block : null;
        set
        {
            if (value is null) Blocks.Remove(location);
            else Blocks[location] = value;
        }
    }

    // --- START: New Greedy Meshing Logic ---

    public IEnumerable<Component> GenerateMeshes()
    {
        var processedLocations = new HashSet<Location>();

        foreach (var startLocation in Blocks.Keys)
        {
            if (processedLocations.Contains(startLocation))
            {
                continue;
            }

            var currentBlock = this[startLocation];

            // If the block is not a solid, flat color block, generate it individually.
            // This prevents complex blocks like stairs or fences from being meshed.
            if (currentBlock is not FlatColorBlock currentFlatBlock || currentBlock is not ISolid)
            {
                foreach (var component in currentBlock.Generate(this, startLocation))
                {
                    yield return component;
                }
                processedLocations.Add(startLocation);
                continue;
            }

            // 1. Expand in X direction (width)
            var width = 1;
            while (true)
            {
                var nextLoc = startLocation + new Location(width, 0, 0);
                if (Blocks.TryGetValue(nextLoc, out var nextBlock) &&
                    nextBlock is FlatColorBlock nextFlatBlock &&
                    nextFlatBlock.Color == currentFlatBlock.Color &&
                    !processedLocations.Contains(nextLoc))
                {
                    width++;
                }
                else
                {
                    break;
                }
            }

            // 2. Expand in Z direction (depth)
            var depth = 1;
            while (true)
            {
                var canExpand = true;
                for (var x = 0; x < width; x++)
                {
                    var nextLoc = startLocation + new Location(x, 0, depth);
                    if (!Blocks.TryGetValue(nextLoc, out var nextBlock) ||
                       !(nextBlock is FlatColorBlock nextFlatBlock) ||
                       nextFlatBlock.Color != currentFlatBlock.Color ||
                       processedLocations.Contains(nextLoc))
                    {
                        canExpand = false;
                        break;
                    }
                }

                if (canExpand)
                {
                    depth++;
                }
                else
                {
                    break;
                }
            }

            // 3. Expand in Y direction (height)
            var height = 1;
            while (true)
            {
                var canExpand = true;
                for (var x = 0; x < width; x++)
                {
                    for (var z = 0; z < depth; z++)
                    {
                        var nextLoc = startLocation + new Location(x, height, z);
                        if (!Blocks.TryGetValue(nextLoc, out var nextBlock) ||
                           !(nextBlock is FlatColorBlock nextFlatBlock) ||
                           nextFlatBlock.Color != currentFlatBlock.Color ||
                           processedLocations.Contains(nextLoc))
                        {
                            canExpand = false;
                            break;
                        }
                    }
                    if (!canExpand) break;
                }

                if (canExpand)
                {
                    height++;
                }
                else
                {
                    break;
                }
            }

            // Mark all blocks in the new cuboid as processed
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    for (var z = 0; z < depth; z++)
                    {
                        processedLocations.Add(startLocation + new Location(x, y, z));
                    }
                }
            }

            // Create one large cube for the entire mesh
            var min = startLocation.ToVector();
            var max = (startLocation + new Location(width - 1, height - 1, depth - 1)).ToVector();

            var centerLocation = (min + max) / 2f;
            var scaleVector = new Vector3(width, height, depth);

            var cube = new Cube(Cube.Style.Edges, BlockToWorld(new Location((int)centerLocation.X, (int)centerLocation.Y, (int)centerLocation.Z)), Vector3.Zero, scaleVector);
            cube.AddBaseColor(currentFlatBlock.Color);

            yield return cube;
        }
    }

    // --- END: New Greedy Meshing Logic ---

    public void CheckBidirectional(Location from, Location to, Action<Block> action)
    {
        if (!(from <= to)) return;
        Check(to, action);
    }

    public void Check(Location to, Action<Block> action)
    {
        Blocks.TryGetValue(to, out var block);
        action(block);
    }

    public void MultiDirectionAction(Action<DirectionContext> action)
    {
        Context.Reset();
        Context.Flip = false;
        action(Context);
        Context.Flip = true;
        action(Context);
    }

    // Modified to accept a list of components
    public void GenerateFile(string path, IEnumerable<Component> components, bool append = false)
    {
        using var output = new StreamWriter(path, append);
        if (!string.IsNullOrWhiteSpace(Header)) output.WriteLine(Header);
        foreach (var component in components)
        {
            if (component == null) continue;
            output.WriteLine(component.ToString());
        }
    }
}