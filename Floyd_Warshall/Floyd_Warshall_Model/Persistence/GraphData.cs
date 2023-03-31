using Floyd_Warshall_Model.Persistence.Graph;

namespace Floyd_Warshall_Model.Persistence
{
    public class GraphData
    {
        public GraphData(GraphBase graph, IEnumerable<VertexData> vertexData)
        {
            Graph = graph;
            VertexDatas = vertexData;
        }

        public GraphBase Graph { get; }
        public IEnumerable<VertexData> VertexDatas { get; }
    }
}
