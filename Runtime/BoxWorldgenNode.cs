// BoxWorldgenNode.cs - MonoBehaviour with a BoxCollider sibling
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BoxWorldgenNode : HierarchicalWorldgenNode
{
    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public Bounds Bbox => new Bounds(transform.position, boxCollider.size);

    public override bool Overlaps(Vector3 position, float radius)
    {
        Vector3 closest = Bbox.ClosestPoint(position);
        return (closest - position).sqrMagnitude <= radius * radius;
    }
}