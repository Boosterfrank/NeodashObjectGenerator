using System.Numerics;
using NeodashObjectGenerator.Gen.Components;

namespace NeodashObjectGenerator.Gen;

public static class ComponentExtensions
{
    // These methods apply to objects that have the ability to glow.
    #region Glow Methods
    
    public static void AddGlowColor<T>(this T t, Vector3 color)
        where T : Component, IGlow
    {
        t.AddParam(new VectorParameter("GlowColor", color));
    }

    public static void NoBand<T>(this T t)
        where T : Component, IGlow
    {
        t.AddBandThickness(0);
    }
    
    public static void AddBandThickness<T>(this T t, float value)
        where T : Component, IGlow
    {
        t.AddParam(new ScalarParameter("BandThickness", value));
    }

    public static void AddBeatModThickness<T>(this T t, float scale)
        where T : Component, IGlow
    {
        t.AddParam(new ScalarParameter("beatModifiesBandThickness", scale));
    }

    public static void AddGlowIntensity<T>(this T t, float intensity)
        where T : Component, IGlow
    {
        t.AddParam(new ScalarParameter("glowIntensity", intensity));
    }
    
    #endregion
}