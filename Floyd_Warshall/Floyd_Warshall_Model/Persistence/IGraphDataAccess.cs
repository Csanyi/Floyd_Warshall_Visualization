using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floyd_Warshall_Model.Persistence
{
    public interface IGraphDataAccess
    {
        Task<Tuple<Graph, IEnumerable<Tuple<Vertex,double,double>>>> LoadAsync(string path);

        Task SaveAsync(string path, Graph graph, IEnumerable<Tuple<Vertex,double,double>> locations);
    }
}
