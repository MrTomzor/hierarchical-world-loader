// WorldLoadingRequestor.cs
using UnityEngine;

public class WorldLoadingRequestor : MonoBehaviour
{
    public int RequiredLevel = 5;
    public float Radius = 50f;

    private void OnEnable()
    {
        HierarchicalWorldLoader.RegisterRequestor(this);
    }

    private void OnDisable()
    {
        HierarchicalWorldLoader.UnregisterRequestor(this);
    }
}