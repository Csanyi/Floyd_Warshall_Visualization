using Floyd_Warshall_Model.Persistence.Graph;

namespace Floyd_Warshall_Model.Persistence
{
    /// <summary>
    /// Type to stor graph data
    /// </summary>
    public class GraphData
    {
        /// <summary>
        /// Constructor of the graph data
        /// </summary>
        /// <param name="graph">Graph</param>
        /// <param name="vertexData">The location of the vertices of the graph</param>
        public GraphData(GraphBase graph, IEnumerable<VertexData> vertexData)
        {
            Graph = graph;
            VertexData = vertexData;
        }

        /// <summary>
        /// Gets the graph
        /// </summary>
        public GraphBase Graph { get; }

        /// <summary>
        /// Gets the vertex data
        /// </summary>
        public IEnumerable<VertexData> VertexData { get; }
    }
}
