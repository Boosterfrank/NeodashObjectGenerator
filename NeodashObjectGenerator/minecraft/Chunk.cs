using System.Text;

namespace NeodashObjectGenerator.Minecraft;

public class Chunk
{
    public TagCompound Root;
    public Coord Coords;
    public int Timestamp;
    public bool Dirty = false;
    public byte CompressionType;
    public byte[] RawData = null;
    public int[] ManualHeightmap = null;

    public override string ToString()
    {
        var sb = new StringBuilder();
        var time = new DateTime(1970, 1, 1).AddSeconds(Timestamp);

        sb.AppendFormat("Chunk [{0}, {1}] {2:M/d/yyyy h:mm:ss tt}{3}{{{3}", Coords.X, Coords.Z, time, Environment.NewLine);
        if (Root != null)
            sb.Append(Root.ToString());
        sb.AppendLine("}");
        return sb.ToString();
    }
}