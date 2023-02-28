using Floyd_Warshall_Model.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floyd_Warshall_Model
{
    using VertexLocation = Tuple<Vertex, double, double>;

    public class GraphModel
    {
        private Graph _graph;
        private IGraphDataAccess _dataAccess;

        public Graph Graph { get { return _graph; } }

        public event EventHandler NewEmptyGraph;
        public event EventHandler<IEnumerable<VertexLocation>> GraphLoaded;

        public GraphModel(IGraphDataAccess dataAccess)
        {
            _graph = new Graph(false);
            _dataAccess = dataAccess;
        }

        public void NewGraph(bool isDirected)
        {
            _graph = new Graph(isDirected);
            OnNewEmptyGraph();
        }

        public async Task LoadAsync(string path)
        {
            if (_dataAccess == null)
            {
                throw new InvalidOperationException("No data acces provided.");
            }

            var v = await _dataAccess.LoadAsync(path);

            _graph = v.Item1;

            OnGraphLoaded(v.Item2);
        }

        public async Task SaveAsync(string path, IEnumerable<VertexLocation> locations)
        {
            if (_dataAccess == null)
            {
                throw new InvalidOperationException("No data acces provided.");
            }

            await _dataAccess.SaveAsync(path, _graph, locations);
        }

        private void OnNewEmptyGraph() => NewEmptyGraph?.Invoke(this, EventArgs.Empty);

        private void OnGraphLoaded(IEnumerable<VertexLocation> locations) => GraphLoaded?.Invoke(this, locations);
    }
}
