using UnityEngine;
using System.Collections.Generic;

public abstract class HierarchicalWorldLoader : MonoBehaviour
{
    [SerializeField] private LayerMask planningLayerMask;
    [SerializeField] private int maxNodesLoadedPerFrame = 5;
    [SerializeField] private float unloadRadiusMultiplier = 1.2f;

    private static readonly List<WorldLoadingRequestor> requestors = new();
    private Dictionary<int, List<IWorldgenNodeListener>> dispatchTable;
    private HierarchicalWorldgenNode rootNode;
    private Queue<(HierarchicalWorldgenNode node, bool load)> loadQueue = new();

    protected abstract Dictionary<int, List<IWorldgenNodeListener>> BuildDispatchTable();
    protected abstract HierarchicalWorldgenNode BuildRootNode();

    private void Awake()
    {
        dispatchTable = BuildDispatchTable();
        rootNode = BuildRootNode();
    }

    private void Update()
    {
        // 1. traverse tree and enqueue load/unload operations
        TraverseNode(rootNode);

        // 2. process queue with per-frame budget
        int processed = 0;
        while (loadQueue.Count > 0 && processed < maxNodesLoadedPerFrame)
        {
            var (node, load) = loadQueue.Dequeue();
            if (load) LoadNode(node);
            else UnloadNode(node);
            processed++;
        }
    }

    private void TraverseNode(HierarchicalWorldgenNode node)
    {
        int requiredLevel = GetRequiredLevelForNode(node);

        // should this node's children exist?
        bool shouldHaveChildren = requiredLevel > node.Level;

        if (shouldHaveChildren && node.Children == null)
        {
            loadQueue.Enqueue((node, true));
        }
        else if (!shouldHaveChildren && node.Children != null)
        {
            loadQueue.Enqueue((node, false));
        }

        // recurse into existing children
        if (node.Children != null)
        {
            foreach (var child in node.Children)
                TraverseNode(child);
        }
    }

    private int GetRequiredLevelForNode(HierarchicalWorldgenNode node)
    {
        int maxRequired = 0;

        foreach (var requestor in requestors)
        {
            if (node.Overlaps(requestor.transform.position, requestor.Radius))
            {
                if (requestor.RequiredLevel > maxRequired)
                    maxRequired = requestor.RequiredLevel;
            }
        }

        return maxRequired;
    }

    // TODO - switch later (with hysteresis)
    /*private int GetRequiredLevelForNode(HierarchicalWorldgenNode node)
    {
        int maxRequired = 0;
        bool alreadyLoaded = node.Children != null;

        foreach (var requestor in requestors)
        {
            // use larger radius if already loaded (hysteresis)
            float radius = alreadyLoaded 
                ? requestor.Radius * unloadRadiusMultiplier 
                : requestor.Radius;

            if (node.Overlaps(requestor.transform.position, radius))
            {
                if (requestor.RequiredLevel > maxRequired)
                    maxRequired = requestor.RequiredLevel;
            }
        }

        return maxRequired;
    }*/

    private void LoadNode(HierarchicalWorldgenNode node)
    {
        if (!dispatchTable.TryGetValue(node.Level, out var modules))
            return;

        foreach (var module in modules)
            module.OnNodeLoaded(node);
    }

    private void UnloadNode(HierarchicalWorldgenNode node)
    {
        // unload children recursively first
        if (node.Children != null)
        {
            foreach (var child in node.Children)
                UnloadNode(child);
            node.Children = null;
        }

        if (!dispatchTable.TryGetValue(node.Level, out var modules))
            return;

        foreach (var module in modules)
            module.OnNodeUnloaded(node);
    }

    public static void RegisterRequestor(WorldLoadingRequestor requestor)
    {
        if (!requestors.Contains(requestor))
            requestors.Add(requestor);
    }

    public static void UnregisterRequestor(WorldLoadingRequestor requestor)
    {
        requestors.Remove(requestor);
    }
}