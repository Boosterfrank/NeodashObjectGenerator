using System.Numerics;

namespace NeodashObjectGenerator.Gen.Components;

// Generates neodash spikes
public class Spikes : ComponentBase, IGlow
{
    public Spikes(Vector3 position, Vector3 rotation, Vector3 scale, int group = 0) : base(position, rotation, scale, group)
    {
        Name = "spikes";
    }
}