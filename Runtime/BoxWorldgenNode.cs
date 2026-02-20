// BoxWorldgenNode.cs
using UnityEngine;

public class BoxWorldgenNode : HierarchicalWorldgenNode
{
    public Bounds Bbox;

    public BoxWorldgenNode(int level, Bounds bbox, int seed, HierarchicalWorldgenNode parent = null)
        : base(level, seed, parent)
    {
        Bbox = bbox;
    }

    public override bool Overlaps(Vector3 position, float radius)
    {
        Vector3 closest = Bbox.ClosestPoint(position);
        return (closest - position).sqrMagnitude <= radius * radius;
    }
}