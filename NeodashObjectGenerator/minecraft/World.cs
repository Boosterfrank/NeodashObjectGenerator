using System.IO.Compression;
using System.Text.RegularExpressions;

// using Ionic.Zlib;

namespace NeodashObjectGenerator.Minecraft;

public class World
{
    public long Seed;
    public long OriginalSeed;
    public string WorldDir;
    public string WorldName;
    private int _version;
    private string _levelDatPath;

    public World(string path)
    {
        TagCompound data;
        _levelDatPath = path;

        using (var level = new FileStream(path, FileMode.Open))
        {
            using (var decompress = new GZipStream(level, CompressionMode.Decompress))
            {
                var mem = new MemoryStream();
                decompress.CopyTo(mem);
                mem.Seek(0, SeekOrigin.Begin);
                data = new TagCompound(mem);
            }
        }

        Seed = (long)data["Data"]["RandomSeed"];
        OriginalSeed = Seed;
        _version = (int)data["Data"]["version"];
        WorldName = (string)data["Data"]["LevelName"];
        WorldDir = Path.GetDirectoryName(path);
    }

    public string GetRegionDirectory(Dimension dim)
    {
        string path;
        switch (dim)
        {
            case Dimension.Overworld:
                path = string.Format("{0}{1}region", WorldDir, Path.DirectorySeparatorChar);
                break;
            case Dimension.Nether:
                path = string.Format("{0}{1}DIM-1{1}region", WorldDir, Path.DirectorySeparatorChar);
                break;
            case Dimension.End:
                path = string.Format("{0}{1}DIM1{1}region", WorldDir, Path.DirectorySeparatorChar);
                break;
            default:
                throw new Exception("Unrecognized dimension.");
        }
            
        if (Directory.Exists(path))
            return path;
        else
        {
            switch (dim)
            {
                case Dimension.Overworld:
                    return string.Format("{0}{1}worlds{1}overworld{1}regions", WorldDir, Path.DirectorySeparatorChar);
                case Dimension.Nether:
                    return string.Format("{0}{1}worlds{1}nether{1}regions", WorldDir, Path.DirectorySeparatorChar);
                case Dimension.End:
                    return string.Format("{0}{1}worlds{1}the_end{1}regions", WorldDir, Path.DirectorySeparatorChar);
                default:
                    throw new Exception("Unrecognized dimension.");
            }
        }
    }

    public string[] GetRegionPaths(Dimension dim = Dimension.Overworld)
    {
        var dir = GetRegionDirectory(dim);
        if (Directory.Exists(dir))
        {
            var regions = new List<string>(Directory.GetFiles(dir, "*.mca", SearchOption.TopDirectoryOnly));
            regions.Sort(CompareRegionNames);
            return regions.ToArray();
        }
        else
            return new string[0];
    }

    private static int CompareRegionNames(string r1, string r2)
    {
        var pattern = new Regex(@"r\.(-?\d+)\.(-?\d+)\.mca");
        var m = pattern.Match(r1);
        var x1 = int.Parse(m.Groups[1].Value);
        var z1 = int.Parse(m.Groups[2].Value);
        m = pattern.Match(r2);
        var x2 = int.Parse(m.Groups[1].Value);
        var z2 = int.Parse(m.Groups[2].Value);

        if (x1 < x2)
            return -1;
        else if (x2 < x1)
            return 1;
        else
        {
            if (z1 < z2)
                return -1;
            else if (z2 < z1)
                return 1;
            else
                return 0;
        }
    }

    public void Write()
    {
        TagCompound data;

        using (var level = new FileStream(_levelDatPath, FileMode.Open))
        {
            using (var decompress = new GZipStream(level, CompressionMode.Decompress))
            {
                var mem = new MemoryStream();
                decompress.CopyTo(mem);
                mem.Seek(0, SeekOrigin.Begin);
                data = new TagCompound(mem);
            }
        }

        ((TagLong)data["Data"]["RandomSeed"]).Payload = Seed;

        using (var level = new FileStream(_levelDatPath, FileMode.Truncate))
        {
            var mem = new MemoryStream();
            var compress = new GZipStream(mem, CompressionMode.Compress);
            data.Write(compress);
            compress.Close();
            var buffer = mem.ToArray();
            level.Write(buffer, 0, buffer.Length);
        }

    }
}