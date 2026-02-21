using UnityEngine;

public static class WorldgenUtils
{
    public static Vector3 RandomPointInBounds(Bounds bounds, System.Random rng)
    {
        return new Vector3(
            (float)(bounds.min.x + rng.NextDouble() * bounds.size.x),
            bounds.center.y,
            (float)(bounds.min.z + rng.NextDouble() * bounds.size.z)
        );
    }

    public static int DeriveChildSeed(int parentSeed, int childIndex, int level)
    {
        int hash = parentSeed;
        hash ^= childIndex * 397;
        hash ^= level * 1000003;
        return hash;
    }
}