using Floyd_Warshall_Model.Graph;

namespace Floyd_Warshall_Model.Persistence
{
    using VertexLocation = Tuple<Vertex, double, double>;
    using GraphData = Tuple<GraphBase, IEnumerable<Tuple<Vertex, double, double>>>;

    public interface IGraphDataAccess
    {
        Task<GraphData> LoadAsync(string path);

        Task SaveAsync(string path, GraphBase graph, IEnumerable<VertexLocation> locations);
    }
}
