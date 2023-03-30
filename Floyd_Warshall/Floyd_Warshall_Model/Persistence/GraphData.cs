using Floyd_Warshall_Model.Persistence.Graph;

namespace Floyd_Warshall_Model.Persistence
{
    public class GraphData
    {
        public GraphData(GraphBase graph, IEnumerable<VertexLocation> vertexLocations)
        {
            Graph = graph;
            VertexLocations = vertexLocations;
        }

        public GraphBase Graph { get; }
        public IEnumerable<VertexLocation> VertexLocations { get; }
    }
}
