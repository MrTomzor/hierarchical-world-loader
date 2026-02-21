// HierarchicalWorldgenNode.cs - now a MonoBehaviour
using UnityEngine;
using System.Collections.Generic;

public abstract class HierarchicalWorldgenNode : MonoBehaviour
{
    public int Level;
    public int Seed;
    public HierarchicalWorldgenNode Parent;
    public List<HierarchicalWorldgenNode> Children;

    public abstract bool Overlaps(Vector3 position, float radius);

    public int DeriveChildSeed(int childIndex)
    {
        return WorldgenUtils.DeriveChildSeed(Seed, childIndex, Level);
    }
}