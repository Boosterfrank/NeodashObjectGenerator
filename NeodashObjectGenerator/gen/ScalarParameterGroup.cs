namespace NeodashObjectGenerator.Gen;

// Represents a scalar parameter, which is grouped together with other scalar parameters
public class ScalarParameter : NamedParameter<float>, IParameterGroup
{
    public ScalarParameter(string name, float data) : base(name, data) { }

    public string Group => "ScalarMaterialParameters";

    public string Value => ToString();
}