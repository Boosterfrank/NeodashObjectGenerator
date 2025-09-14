using System.IO.Compression;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

// using Ionic.Zlib;

namespace NeodashObjectGenerator.Minecraft;

public class RegionFile
{
    public Chunk[,] Chunks;
    public Coord Coords;
    public string Path;
    public bool Dirty = false;

    private int _taskCount = 0;
    private ManualResetEvent _signal = null;

    public RegionFile()
    {
        Chunks = new Chunk[32, 32];
    }

    public RegionFile(string path)
    {
        Path = path;
        Read(Path);
    }

    public RegionFile(string path, int startX, int endX, int startZ, int endZ)
    {
        Path = path;
        Read(Path, startX, endX, startZ, endZ);
    }


    //DO NOT write region if reading less than the entire thing
    public void Read(string path)
    {
        Read(path, 0, 31, 0, 31);
    }

    //http://www.minecraftwiki.net/wiki/Region_file_format
    public void Read(string path, int startX, int endX, int startZ, int endZ)
    {
        Chunks = new Chunk[32, 32];
        var m = Regex.Match(path, @"r\.(-?\d+)\.(-?\d+)\.mca");
        Coords = new Coord(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value));

        if (!File.Exists(path))
            return;

        _signal = new ManualResetEvent(false);
        _taskCount = (endX - startX + 1) * (endZ - startZ + 1);

        var header = new byte[8192];
            
        using (var file = MemoryMappedFile.CreateFromFile(path, FileMode.Open))
        {
            using (var reader = file.CreateViewStream())
            {
                reader.Read(header, 0, 8192);

                for (var chunkZ = startZ; chunkZ <= endZ; chunkZ++)
                {
                    for (var chunkX = startX; chunkX <= endX; chunkX++)
                    {
                        var c = new Chunk();
                        c.Coords = new Coord(Coords).RegionToChunk().Add(chunkX, chunkZ);

                        var i = 4 * (chunkX + chunkZ * 32);

                        var temp = new byte[4];
                        temp[0] = 0;
                        Array.Copy(header, i, temp, 1, 3);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(temp);
                        var offset = ((long)BitConverter.ToInt32(temp, 0)) * 4096;
                        var length = header[i + 3] * 4096;

                        temp = new byte[4];
                        Array.Copy(header, i + 4096, temp, 0, 4);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(temp);
                        c.Timestamp = BitConverter.ToInt32(temp, 0);

                        if (offset == 0 && length == 0)
                        {
                            Chunks[chunkX, chunkZ] = c;
                            if (Interlocked.Decrement(ref _taskCount) == 0)
                                _signal.Set();
                            continue;
                        }

                        reader.Seek(offset, SeekOrigin.Begin);

                        temp = new byte[4];
                        reader.Read(temp, 0, 4);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(temp);
                        var exactLength = BitConverter.ToInt32(temp, 0);

                        c.CompressionType = (byte)reader.ReadByte();

                        c.RawData = new byte[exactLength - 1];
                        reader.Read(c.RawData, 0, exactLength - 1);

                        Chunks[chunkX, chunkZ] = c;

                        ThreadPool.QueueUserWorkItem(Decompress, c);

                    }
                }
                reader.Close();
            }
        }
        _signal.WaitOne();
        _signal.Dispose();
        _signal = null;
    }

    private void Decompress(object state)
    {            
        var c = (Chunk)state;

        if (c.CompressionType == 1) //GZip
        {
            var decompress = new GZipStream(new MemoryStream(c.RawData), CompressionMode.Decompress);
            var mem = new MemoryStream();
            decompress.CopyTo(mem);
            mem.Seek(0, SeekOrigin.Begin);
            c.Root = new TagCompound(mem);
        }
        else if (c.CompressionType == 2) //Zlib
        {
            var inflater = new InflaterInputStream(new MemoryStream(c.RawData));
            var mem = new MemoryStream();
            inflater.CopyTo(mem);
            mem.Seek(0, SeekOrigin.Begin);
            c.Root = new TagCompound(mem);
        }
        else
        {
            throw new Exception("Unrecognized compression type");
        }

        if (Interlocked.Decrement(ref _taskCount) == 0)
            _signal.Set();
    }

    //DO NOT write region if reading less than the entire thing
    public void Write(bool force = false)
    {
        Write(Path, force);
    }

    public void Write(string path, bool force = false)
    {
        if (!force && !Dirty)
            return;
        var header = new byte[8192];
        Array.Clear(header, 0, 8192);

        var sectorOffset = 2;
        using (var file = new BinaryWriter(File.Exists(path) ? File.Open(path, FileMode.Truncate) : File.Open(path, FileMode.Create)))
        {
            file.Write(header, 0, 8192);

            for (var chunkX = 0; chunkX < 32; chunkX++)
            {
                for (var chunkZ = 0; chunkZ < 32; chunkZ++)
                {
                    var c = Chunks[chunkX, chunkZ];
                    if (c == null)
                        continue;
                    
                    var i = 4 * (chunkX + chunkZ * 32);

                    var temp = BitConverter.GetBytes(c.Timestamp);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(temp);
                    Array.Copy(temp, 0, header, i + 4096, 4);

                    if (c.Root == null)
                    {
                        Array.Clear(temp, 0, 4);
                        Array.Copy(temp, 0, header, i, 4);
                        continue;
                    }

                    temp = BitConverter.GetBytes(sectorOffset);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(temp);
                    Array.Copy(temp, 1, header, i, 3);

                    if (c.RawData == null || force || c.Dirty)
                    {
                        //this is the performance bottleneck when doing 1024 chunks in a row;
                        //trying to only do when necessary
                        var mem = new MemoryStream();
                        var deflate = new DeflaterOutputStream(mem);
                        // ZlibStream zlib = new ZlibStream(mem, CompressionMode.Compress);
                        c.Root.Write(deflate);
                        deflate.Close();
                        c.RawData = mem.ToArray();
                        c.CompressionType = 2;
                    }

                    temp = BitConverter.GetBytes(c.RawData.Length + 1);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(temp);

                    file.Write(temp, 0, 4);
                    file.Write(c.CompressionType);
                    file.Write(c.RawData, 0, c.RawData.Length);

                    var padding = new byte[(4096 - ((c.RawData.Length + 5) % 4096))];
                    Array.Clear(padding, 0, padding.Length);
                    file.Write(padding);

                    header[i + 3] = (byte)((c.RawData.Length + 5) / 4096 + 1);
                    sectorOffset += (c.RawData.Length + 5) / 4096 + 1;
                    c.Dirty = false;
                }
            }

            file.Seek(0, SeekOrigin.Begin);
            file.Write(header, 0, 8192);
            file.Flush();
            file.Close();
            Dirty = false;
        }

    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendFormat("Region [{0}, {1}]{2}{{{2}", Coords.X, Coords.Z, Environment.NewLine);
        foreach (var c in Chunks)
            sb.Append(c.ToString());
        sb.AppendLine("}");
        return sb.ToString();
    }

    public static string ToString(string path)
    {
        var m = Regex.Match(path, @"r\.(-?\d+)\.(-?\d+)\.mca");
        var c = new Coord(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value)).RegionToAbsolute();
        var c2 = new Coord(int.Parse(m.Groups[1].Value) + 1, int.Parse(m.Groups[2].Value) + 1).RegionToAbsolute();
        return string.Format("Region {0}, {1} :: ({2}, {3}) to ({4}, {5})", m.Groups[1].Value, m.Groups[2].Value, c.X, c.Z, c2.X - 1, c2.Z - 1);
    }
}