using System.Numerics;

namespace NeodashObjectGenerator.Gen;

// Represents a vector parameter, which is grouped with other vector parameters
public class VectorParameter : NamedParameter<Vector3>, IParameterGroup
{
    public VectorParameter(string name, Vector3 data) : base(name, data) { }

    public string Group => "VectorMaterialParameters";

    public string Value => ToString();

    public override string ToString() => $"{Name},{Data.X},{Data.Y},{Data.Z}";
}