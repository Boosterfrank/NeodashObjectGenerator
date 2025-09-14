using System.Numerics;
using System.Text;
using NeodashObjectGenerator.Gen;
using NeodashObjectGenerator.Gen.Components;

namespace NeodashObjectGenerator.MCConverter;

// Class that assists with structure definitions.
// Can read level CSV data and convert into C# code that generates the structure.
// The output from this class is intended to be put into a Block's Generate method.
public class StructureGenerator
{
    // The center of the "block" where the structure is built
    public Vector3 Origin;
    
    // Generated code
    public List<string> Code;

    // Variable name counters
    private Dictionary<string, int> _baseNames;

    // Usually when building a structure you build it next to another block for reference.
    // Origin is the location of the reference block, and relative is the relative location
    // of the structure to the reference block.
    public StructureGenerator(Vector3 origin, Location relative)
    {
        Origin = origin + relative.McToNeodash() * Sizes.BlockSize;
        Code = new List<string>();
        _baseNames = new Dictionary<string, int>();
        
        Code.Add("var block = world.BlockToWorld(location);" + Environment.NewLine + "var group = world.NextGroup();");
    }

    private Vector3 ReadVector(ReadOnlySpan<string> span)
    {
        return new Vector3(float.Parse(span[0]), float.Parse(span[1]), float.Parse(span[2]));
    }

    // Get a variable name with an index appended to it
    private string Var(string baseName)
    {
        _baseNames.TryGetValue(baseName, out var index);
        var result = baseName + index;
        _baseNames[baseName] = index + 1;
        return result;
    }

    // Generates a declaration for a given component type
    private string MakeConstructor(string type, string position, string rotation, string scale, out string variable)
    {
        variable = Var(type);
        var resultType = type switch
        {
            "multiCube" or "multiCube2" => typeof(Cube),
            "peg1" => typeof(Peg),
            "sawTrap1" => typeof(Saw),
            "spikes" => typeof(Spikes),
            _ => null
        };
        if (resultType == null) return "null";

        var method = typeof(ComponentBase).GetMethod("ConstructorString");
        var genericMethod = method?.MakeGenericMethod(resultType);
        var cons = (string) genericMethod?.Invoke(null, new object[] {type, position, rotation, scale, "group"});
        return $"var {variable} = {cons};";
    }

    // Add the given object to the structure.
    // Adds code that will generate the object relative to the current block.
    public void Add(string csv)
    {
        var lines = csv.Split(Environment.NewLine);
        if (lines.Length > 1)
        {
            foreach (var line in lines)
            {
                Add(line);
            }
            return;
        }
        
        var parts = csv.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var name = parts[0];
        var position = (ReadVector(parts.AsSpan(1..4)) - Origin) / Sizes.BlockSize;
        var rotation = ReadVector(parts.AsSpan(4..7));
        var scale = ReadVector(parts.AsSpan(7..10));

        var builder = new StringBuilder();
        var pos = position == Vector3.Zero ? "block" : $"block + world.Shift({position.ComponentString()})";
        builder.Append(MakeConstructor(name, pos, rotation.ConstructorString(), scale.ConstructorString(), out var variable));
        AddParameters(parts, 11, variable, builder);

        builder.Append(Environment.NewLine).Append($"yield return {variable};");
        
        Code.Add(builder.ToString());
    }

    // Add methods calls to match certain properties.
    public void AddParameters(string[] parts, int start, string variable, StringBuilder builder)
    {
        for (var i = start; i < parts.Length; i++)
        {
            var part = parts[i];
            if (part == "BandThickness")
            {
                builder.Append(Environment.NewLine).Append($"{variable}.AddBandThickness({float.Parse(parts[i + 1])}f);");
                i++;
            }
            else if (part == "glowIntensity")
            {
                builder.Append(Environment.NewLine).Append($"{variable}.AddGlowIntensity({float.Parse(parts[i + 1])}f);");
                i++;
            }
            else if (part == "BaseColor")
            {
                builder.Append(Environment.NewLine)
                    .Append($"{variable}.AddBaseColor({ReadVector(parts.AsSpan(i + 1, 3)).ConstructorString()});");
                i += 3;
            }
            else if (part == "GlowColor")
            {
                builder.Append(Environment.NewLine)
                    .Append($"{variable}.AddGlowColor({ReadVector(parts.AsSpan(i + 1, 3)).ConstructorString()});");
                i += 3;
            }
        }
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine + Environment.NewLine, Code);
    }
}