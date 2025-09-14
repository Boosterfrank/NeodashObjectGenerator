using System.Numerics;

namespace NeodashObjectGenerator.Gen.Components;

// Generates a neodash cube
public class Cube : ComponentBase, IGlow
{
    public Cube(Style style, Vector3 position, Vector3 rotation, Vector3 scale, int group = 0) : base(position, rotation, scale, group)
    {
        Name = style switch
        {
            Style.Midline => "multiCube",
            Style.Edges => "multiCube2",
            _ => "invalid",
        };
    }

    public enum Style
    {
        Midline,
        Edges,
    }

    // Special implementation of MakeConstructor which adds the two cube styles to the arguments
    public static string MakeConstructor(string type, string position, string rotation, string scale, string group)
    {
        var style = type switch
        {
            "multiCube" => Style.Midline,
            "multiCube2" => Style.Edges,
            _ => (Style) 0,
        };
        
        var cons = ComponentBase.MakeConstructor<Cube>(type, position, rotation, scale, group);
        var open = cons.IndexOf('(') + 1;
        return $"{cons[..open]}Cube.Style.{Enum.GetName(style)}, {cons[open..]}";
    }
}