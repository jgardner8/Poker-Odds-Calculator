using System;
using System.Threading;

/// <summary>
/// Courtesy of Jon Skeet
/// http://stackoverflow.com/questions/1785744/how-do-i-seed-a-random-class-to-avoid-getting-duplicate-random-values
/// </summary>
public static class RandomHelper
{
    private static int _seedCounter = new Random().Next();

    [ThreadStatic]
    private static Random rng;

    public static Random Instance
    {
        get
        {
            if (rng == null)
            {
                int seed = Interlocked.Increment(ref _seedCounter);
                rng = new Random(seed);
            }
            return rng;
        }
    }
}