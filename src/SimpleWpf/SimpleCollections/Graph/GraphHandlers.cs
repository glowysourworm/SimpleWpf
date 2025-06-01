using SimpleWpf.SimpleCollections.Graph.Interface;

namespace SimpleWpf.SimpleCollections.Graph
{
    /// <summary>
    /// Used to provide a distance metric to the graph nodes. This can be used to calculate edge weight.
    /// </summary>
    public delegate double GraphDistanceDelegate(IGraphNode node1, IGraphNode node2);
}
