using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floyd_Warshall_Model.Persistence
{
    using VertexLocation = Tuple<Vertex, double, double>;
    using GraphData = Tuple<Graph, IEnumerable<Tuple<Vertex, double, double>>>;

    public interface IGraphDataAccess
    {
        Task<GraphData> LoadAsync(string path);

        Task SaveAsync(string path, Graph graph, IEnumerable<VertexLocation> locations);
    }
}
