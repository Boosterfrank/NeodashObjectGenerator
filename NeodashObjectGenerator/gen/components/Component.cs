using System.Numerics;
using System.Text;

namespace NeodashObjectGenerator.Gen.Components;

// Represents a neodash object
// Objects typically follow the format name,data,parameters,
public class Component
{
    public string Name { get; init; } = null!;

    public List<string> Data { get; }
    
    public List<IParameter> Parameters { get; }

    public Component()
    {
        Data = new List<string>();
        Parameters = new List<IParameter>();
    }

    // Add an object to the data section
    public Component Add(object o)
    {
        Data.Add(o.ToString() ?? "null");
        return this;
    }

    // Add a vector to the data section
    public Component Add(Vector3 v)
    {
        Data.Add($"{v.X},{v.Y},{v.Z}");
        return this;
    }

    public Component AddParam(IParameter parameter)
    {
        Parameters.Add(parameter);
        return this;
    }

    public override string ToString()
    {
        var b = new StringBuilder();
        b.Append(Name).Append(',');
        
        // Add all data
        foreach (var s in Data)
        {
            b.Append(s).Append(',');
        }
        
        // Add parameters
        foreach (var group in Parameters.GroupBy(p => p is IParameterGroup))
        {
            if (group.Key)
            {
                // Parameters are typically grouped under a category.
                // For example vector parameters are listed after the
                // "VectorMaterialParameters" token.
                foreach (var paramGroup in group.GroupBy(p => ((IParameterGroup) p).Group))
                {
                    b.Append(paramGroup.Key).Append(',');
                    foreach (var parameter in paramGroup)
                    {
                        b.Append(parameter.Value).Append(',');
                    }
                }
            }
            else
            {
                foreach (var parameter in group)
                {
                    b.Append(parameter.Value).Append(',');
                }
            }
        }

        return b.ToString();
    }
}