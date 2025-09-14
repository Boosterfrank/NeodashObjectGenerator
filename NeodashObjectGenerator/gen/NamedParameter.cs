namespace NeodashObjectGenerator.Gen;

// Represents a parameter that is a name followed by a value
public class NamedParameter<T>
{
    public string Name { get; }
    
    public T Data { get; }

    public NamedParameter(string name, T data)
    {
        Name = name;
        Data = data;
    }

    public override string ToString() => $"{Name},{Data}";
}