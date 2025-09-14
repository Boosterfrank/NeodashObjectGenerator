using System.Numerics;
using System.Reflection;

namespace NeodashObjectGenerator.Gen.Components;

// Represents a simple component with a position, rotation, and scale to start with.
public abstract class ComponentBase : Component
{
    public ComponentBase(Vector3 position, Vector3 rotation, Vector3 scale, int group = 0)
    {
        Add(position);
        Add(rotation);
        Add(scale);
        // Add($"grp {group}");
    }
    
    public void AddBaseColor(Vector3 color) => AddParam(new VectorParameter("BaseColor", color));

    // Used by the StructureGenerator, generates a constructor for the given type.
    // This searches for a MakeConstructor(string, string, string, string, string) method in the given class.
    // It will search the parent classes if one is not found.
    public static string ConstructorString<T>(string type, string position, string rotation, string scale, string group)
    {
        MethodInfo info = null;
        var currentType = typeof(T);
        while (currentType != null)
        {
            info = currentType.GetMethod("MakeConstructor", BindingFlags.Public | BindingFlags.Static, new[] {typeof(string), typeof(string), typeof(string), typeof(string), typeof(string)});
            if (info != null) break;
            currentType = currentType.BaseType;
        }
        if (info?.IsGenericMethod == true)
        {
            info = info.MakeGenericMethod(typeof(T));
        }
        return (string) info?.Invoke(null, new object[] {type, position, rotation, scale, group});
    }
    
    // Generate a simple constructor string
    private static string MakeConstructor(string className, string type, string position, string rotation, string scale, string group)
    {
        return $"new {className}({position}, {rotation}, {scale}, {group})";
    }

    // Generate a constructor string from a known type
    public static string MakeConstructor<T>(string type, string position, string rotation, string scale, string group)
    {
        return MakeConstructor(typeof(T).Name, type, position, rotation, scale, group);
    }
}