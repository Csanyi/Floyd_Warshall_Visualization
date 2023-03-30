using Floyd_Warshall_Model.Model.Graph;

namespace Floyd_Warshall_Model.Model
{
    public class GraphData
    {
        public GraphData(GraphBase graph, IEnumerable<VertexLocation> vertexLocations)
        {
            Graph = graph;
            VertexLocations = vertexLocations;
        }

        public GraphBase Graph { get; private set; }
        public IEnumerable<VertexLocation> VertexLocations { get; private set; }
    }
}
