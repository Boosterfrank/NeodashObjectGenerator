namespace NeodashObjectGenerator.Minecraft;

//http://docs.oracle.com/javase/6/docs/api/java/util/Random.html
//http://stackoverflow.com/questions/2147524/c-java-number-randomization
public class JavaRandom
{
    private long _seed;
    private bool _haveNextNextGaussian;
    private double _nextNextGaussian;

    public JavaRandom()
    {
        SetSeed(Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds));
    }

    public JavaRandom(long s)
    {
        SetSeed(s);
    }


    protected int Next(int bits)
    {
        _seed = (_seed * 0x5DEECE66DL + 0xBL) & ((1L << 48) - 1);
        return (int)(_seed >> (48 - bits));
    }

    public bool NextBoolean()
    {
        return Next(1) != 0;
    }

    public void NextBytes(byte[] bytes)
    {
        for (var i = 0; i < bytes.Length; )
        for (int rnd = NextInt(), n = Math.Min(bytes.Length - i, 4); n-- > 0; rnd >>= 8)
            bytes[i++] = (byte)rnd;
    }

    public double NextDouble()
    {
        return (((long)Next(26) << 27) + Next(27)) / (double)(1L << 53);
    }

    public float NextFloat()
    {
        return Next(24) / ((float)(1 << 24));
    }

    public double NextGaussian()
    {
        if (_haveNextNextGaussian)
        {
            _haveNextNextGaussian = false;
            return _nextNextGaussian;
        }
        else
        {
            double v1, v2, s;
            do
            {
                v1 = 2 * NextDouble() - 1;   // between -1.0 and 1.0
                v2 = 2 * NextDouble() - 1;   // between -1.0 and 1.0
                s = v1 * v1 + v2 * v2;
            } while (s >= 1 || s == 0);
            var multiplier = Math.Sqrt(-2 * Math.Log(s) / s);
            _nextNextGaussian = v2 * multiplier;
            _haveNextNextGaussian = true;
            return v1 * multiplier;
        }
    }

    public int NextInt()
    {
        return Next(32);
    }

    public int NextInt(int n)
    {
        if (n <= 0)
            throw new ArgumentException("n must be positive");

        if ((n & -n) == n)  // i.e., n is a power of 2
            return (int)((n * (long)Next(31)) >> 31);

        int bits, val;
        do
        {
            bits = Next(31);
            val = bits % n;
        } while (bits - val + (n - 1) < 0);
        return val;
    }

    public long NextLong()
    {
        return ((long)Next(32) << 32) + Next(32);
    }

    public void SetSeed(long s)
    {
        _seed = (s ^ 0x5DEECE66DL) & ((1L << 48) - 1);
        _haveNextNextGaussian = false;
    }
}