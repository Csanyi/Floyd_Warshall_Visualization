using Floyd_Warshall_Model.Persistence.Graph;

namespace Floyd_Warshall_Model.Persistence
{
    public interface IGraphDataAccess
    {
        Task<GraphData> LoadAsync(string path);

        Task SaveAsync(string path, GraphBase graph, IEnumerable<VertexLocation> locations);
    }
}
