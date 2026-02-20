// HierarchicalWorldgenNode.cs
using UnityEngine;
using System.Collections.Generic;

public abstract class HierarchicalWorldgenNode
{
    public int Level;
    public int Seed;
    public HierarchicalWorldgenNode Parent;
    public List<HierarchicalWorldgenNode> Children;

    protected HierarchicalWorldgenNode(int level, int seed, HierarchicalWorldgenNode parent = null)
    {
        Level = level;
        Seed = seed;
        Parent = parent;
        Children = null;
    }

    public abstract bool Overlaps(Vector3 position, float radius);

    public int DeriveChildSeed(int childIndex)
    {
        int hash = Seed;
        hash ^= childIndex * 397;
        hash ^= Level * 1000003;
        return hash;
    }
}