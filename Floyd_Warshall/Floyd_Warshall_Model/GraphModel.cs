using Floyd_Warshall_Model.Persistence;
using Floyd_Warshall_Model.Graph;

namespace Floyd_Warshall_Model
{
    using VertexLocation = Tuple<Vertex, double, double>;

    public class GraphModel
    {
        private GraphBase _graph;
        private IGraphDataAccess _dataAccess;

        public GraphBase Graph { get { return _graph; } }

        public event EventHandler NewEmptyGraph;
        public event EventHandler<IEnumerable<VertexLocation>> GraphLoaded;

        public GraphModel(IGraphDataAccess dataAccess)
        {
            _graph = new UndirectedGraph();
            _dataAccess = dataAccess;
        }

        public void NewGraph(bool isDirected)
        {
            if (isDirected)
            {
                _graph = new DirectedGraph();
            }
            else
            {
                _graph = new UndirectedGraph();
            }

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
