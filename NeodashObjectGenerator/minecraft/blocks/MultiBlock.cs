using System.Drawing;
using System.Numerics;
using NeodashObjectGenerator.Gen.Components;
using NeodashObjectGenerator.MCConverter;

namespace NeodashObjectGenerator.Minecraft.Blocks;

// Will generate a full block from various layers
public class MultiBlock : Block, ISolid
{
    public List<Vector3> Colors;

    public MultiBlock()
    {
        Colors = new List<Vector3>();
    }

    public MultiBlock(Vector3 color) : this()
    {
        Colors.Add(color);
    }

    public void Add(Vector3 color) => Colors.Add(color);

    public static Color GetColor(char ch)
    {
        return ch switch
        {
            'R' => Color.Red,
            'O' => Color.Orange,
            'Y' => Color.Yellow,
            'G' => Color.Green,
            'B' => Color.Blue,
            'I' => Color.Indigo,
            'V' => Color.Violet,
            _ => Color.Black
        };
    }

    public override IEnumerable<Component> Generate(SimpleWorld world, Location location)
    {
        var group = world.NextGroup();

        var height = 1f / Colors.Count;
        for (var i = 0; i < Colors.Count; i++)
        {
            var cube = world.MakePartialCube(location, new Vector3(0, 0, i * height), new Vector3(1, 1, (i + 1) * height), group);
            cube.AddBaseColor(Colors[i]);
            yield return cube;
        }
    }
}