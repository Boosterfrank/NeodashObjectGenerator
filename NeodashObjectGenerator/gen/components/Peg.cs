using System.Numerics;

namespace NeodashObjectGenerator.Gen.Components;

// Generates a neodash peg
public class Peg : ComponentBase, IGlow
{
    public Peg(Vector3 position, Vector3 rotation, Vector3 scale, int group = 0) : base(position, rotation, scale, group)
    {
        Name = "peg1";
    }
}