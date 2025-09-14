using System.Numerics;

namespace NeodashObjectGenerator.Gen.Components;

// Generates a neodash saw
public class Saw : ComponentBase, IGlow
{
    public Saw(Vector3 position, Vector3 rotation, Vector3 scale, int group = 0) : base(position, rotation, scale, group)
    {
        Name = "sawTrap1";
    }
}