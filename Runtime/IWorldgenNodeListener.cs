public interface IWorldgenNodeListener
{
    void OnNodeLoaded(HierarchicalWorldgenNode node);
    void OnNodeUnloaded(HierarchicalWorldgenNode node);
}
