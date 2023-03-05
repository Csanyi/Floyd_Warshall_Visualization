using Floyd_Warshall_Model.Persistence;
using Floyd_Warshall_Model.Graph;

namespace Floyd_Warshall_Model
{
    using VertexLocation = Tuple<Vertex, double, double>;

    public class GraphModel
    {
        private GraphBase _graph;
        private FloydWarshall _floydWarshall;
        private readonly IGraphDataAccess _dataAccess;

        public GraphBase Graph { get { return _graph; } }

        public bool IsRunning { get { return _floydWarshall != null && _floydWarshall.IsRunnging; } }

        public int? K { get { return _floydWarshall?.K; } }

        public event EventHandler NewEmptyGraph;
        public event EventHandler<IEnumerable<VertexLocation>> GraphLoaded;
        public event EventHandler<Tuple<int[,], int[,]>> AlgorithmStarted;
        public event EventHandler<Tuple<int[,], int[,]>> AlgorithmStepped;
        public event EventHandler AlgorithmEnded;
        public event EventHandler AlgorithmStopped;

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

        public void StartAlgorithm()
        {
            _floydWarshall = new FloydWarshall(_graph.ToAdjacencyMatrix());
            OnAlhorithmStarted(_floydWarshall.D, _floydWarshall.Pi);
        }

        public void StepAlgorithm()
        {
            if (_floydWarshall.NextStep())
            {
                OnAlgorithmStepped(_floydWarshall.D, _floydWarshall.Pi);
            }
            else
            {
                OnAlhorithmEnded();
            }
        }

        public void StopAlgorithm()
        {
            _floydWarshall = null;
            OnAlhorithmStopped();
        }

        private void OnNewEmptyGraph() => NewEmptyGraph?.Invoke(this, EventArgs.Empty);

        private void OnGraphLoaded(IEnumerable<VertexLocation> locations) => GraphLoaded?.Invoke(this, locations);

        private void OnAlhorithmStarted(int[,] d, int[,] pi) => AlgorithmStarted?.Invoke(this, new Tuple<int[,], int[,]>(d, pi));

        private void OnAlgorithmStepped(int[,] d, int[,] pi) => AlgorithmStepped?.Invoke(this, new Tuple<int[,], int[,]>(d, pi));

        private void OnAlhorithmEnded() => AlgorithmEnded?.Invoke(this, EventArgs.Empty);

        private void OnAlhorithmStopped() => AlgorithmStopped?.Invoke(this, EventArgs.Empty);
    }
}
