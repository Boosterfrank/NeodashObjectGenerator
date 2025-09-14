namespace NeodashObjectGenerator.Gen;

// Represents a parameter that is grouped under a common category
public interface IParameterGroup : IParameter
{
    string Group { get; }
}