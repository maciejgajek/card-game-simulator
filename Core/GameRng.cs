public class GameRng
{
    private uint state;

    public int Seed { get; }

    public GameRng(int seed)
    {
        Seed = seed;
        state = unchecked((uint)seed);
        if (state == 0) state = 1;
    }

    public uint NextUInt()
    {
        state ^= state << 13;
        state ^= state >> 17;
        state ^= state << 5;
        return state;
    }

    public int Next(int maxExclusive)
    {
        if (maxExclusive <= 0) return 0;
        return (int)(NextUInt() % (uint)maxExclusive);
    }

    public int Next(int minInclusive, int maxExclusive)
    {
        if (maxExclusive <= minInclusive) return minInclusive;
        return minInclusive + Next(maxExclusive - minInclusive);
    }

    public float NextFloat()
    {
        return (NextUInt() >> 8) / 16777216f;
    }

    public GameRng Clone()
    {
        var clone = new GameRng(Seed);
        clone.state = state;
        return clone;
    }
}
