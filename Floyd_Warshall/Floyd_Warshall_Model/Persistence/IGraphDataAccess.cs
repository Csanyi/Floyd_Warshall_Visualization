using Floyd_Warshall_Model.Persistence.Graph;

namespace Floyd_Warshall_Model.Persistence
{
    /// <summary>
    /// Inteface to manage the graph data access
    /// </summary>
    public interface IGraphDataAccess
    {
        /// <summary>
        /// Loads graph
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>The loaded graph data</returns>
        Task<GraphData> LoadAsync(string path);

        /// <summary>
        /// Saves graph
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="graph">Graph to save</param>
        /// <param name="locations">Locations of the verteces</param>
        Task SaveAsync(string path, GraphBase graph, IEnumerable<VertexData> locations);
    }
}
