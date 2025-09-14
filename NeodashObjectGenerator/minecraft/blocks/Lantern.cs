using System.Numerics;
using NeodashObjectGenerator.Gen;
using NeodashObjectGenerator.Gen.Components;
using NeodashObjectGenerator.MCConverter;

namespace NeodashObjectGenerator.Minecraft.Blocks;

// Generate a stylized lantern
public class Lantern : Block
{
    public bool Hanging { get; }

    public Lantern(bool hanging)
    {
        Hanging = hanging;
    }

    public override IEnumerable<Component> Generate(SimpleWorld world, Location location)
    {
        var group = world.NextGroup();
        var color = Color.Rgb(72, 79, 99);
        var pixelShift = Pixel(0, 0, Hanging ? 3 : -3);
        
        var baseCube = world.MakePartialCube(location, Pixel(5, 5, 3) + pixelShift, Pixel(11, 11, 10) + pixelShift, group);
        baseCube.AddBaseColor(color);
        yield return baseCube;

        var top = world.MakePartialCube(location, Pixel(6, 6, 10) + pixelShift, Pixel(10, 10, 12) + pixelShift, group);
        top.AddBaseColor(color);
        yield return top;

        var hanger = world.MakePartialCube(location, Pixel(7, 7, 12) + pixelShift, Pixel(9, 9, 13) + pixelShift, group);
        hanger.AddBaseColor(color);
        yield return hanger;
        
        var lightX = world.MakePartialCube(location, new Vector3(90, 0, 0), "X", Pixel(5 - 1 / 32f, 5, 4) + pixelShift, Pixel(11 + 1 / 32f, 11, 9) + pixelShift, group, Cube.Style.Midline);
        lightX.AddBandThickness(0.69f);
        lightX.AddBaseColor(color);
        lightX.AddGlowColor(new Vector3(0.345904f, 0.105958f, 0.007168f));
        lightX.AddBeatModThickness(0.11f);
        yield return lightX;
        
        var lightY = world.MakePartialCube(location, new Vector3(90, 0, 90), "ZX", Pixel(5, 5 - 1 / 32f, 4) + pixelShift, Pixel(11, 11 + 1 / 32f, 9) + pixelShift, group, Cube.Style.Midline);
        lightY.AddBandThickness(0.69f);
        lightY.AddBaseColor(color);
        lightY.AddGlowColor(new Vector3(0.345904f, 0.105958f, 0.007168f));
        lightY.AddBeatModThickness(0.11f);
        yield return lightY;
    }

    public static Block Read(string name, TagCompound tag)
    {
        return new Lantern((string) tag.Prop("hanging") == "true") {Name = name};
    }
}