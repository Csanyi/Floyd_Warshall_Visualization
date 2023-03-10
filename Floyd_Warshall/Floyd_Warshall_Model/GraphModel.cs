using Floyd_Warshall_Model.Persistence;
using Floyd_Warshall_Model.Graph;

namespace Floyd_Warshall_Model
{
    using VertexLocation = Tuple<Vertex, double, double>;

    public class GraphModel
    {
        private GraphBase _graph;
        private FloydWarshall? _floydWarshall;
        private readonly IGraphDataAccess _dataAccess;

        public bool IsDirected => _graph.IsDirected;

        public bool IsAlgorithmRunning => _floydWarshall != null && _floydWarshall.IsRunnging;

        public bool IsAlgorthmInitialized => _floydWarshall != null;

        public int? K  =>  _floydWarshall?.K;

        public event EventHandler? NewEmptyGraph;
        public event EventHandler<IEnumerable<VertexLocation>>? GraphLoaded;
        public event EventHandler? VertexAdded;
        public event EventHandler? VertexRemoved;
        public event EventHandler<Tuple<int[,], int[,]>>? AlgorithmStarted;
        public event EventHandler<Tuple<int[,], int[,]>>? AlgorithmStepped;
        public event EventHandler? AlgorithmEnded;
        public event EventHandler? AlgorithmStopped;

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

        public void AddVertex(Vertex v)
        {
            _graph.AddVertex(v);
            OnVertexAdded();
        }

        public void RemoveVertex(Vertex v)
        {
            _graph.RemoveVertex(v);
            OnVertexRemoved();
        }

        public void AddEdge(Vertex from, Vertex to, short weight) => _graph.AddEdge(from, to, weight);

        public void RemoveEdge(Vertex from, Vertex to) => _graph.RemoveEdge(from, to);

        public Edge? GetEdge(Vertex from, Vertex to) => _graph.GetEdge(from, to);

        public short GetWeight(Vertex from, Vertex to) => _graph.GetWeight(from, to);

        public void UpdateWeight(Vertex from, Vertex to, short weirht) => _graph.UpdateWeight(from, to, weirht);

        public int GetVertexCount() => _graph.GetVertexCount();

        public List<Edge> GetEdges() => _graph.GetEdges();

        public List<int> GetVertexIds() => _graph.GetVertexIds();

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
            _floydWarshall = new FloydWarshall(_graph.ToAdjacencyMatrix(), _graph.GetVertexIds() );
            OnAlhorithmStarted(_floydWarshall.D, _floydWarshall.Pi);
        }

        public void StepAlgorithm()
        {
            if( _floydWarshall == null ) { return; }

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

        private void OnVertexAdded() => VertexAdded?.Invoke(this, EventArgs.Empty);

        private void OnVertexRemoved() => VertexRemoved?.Invoke(this, EventArgs.Empty);

        private void OnAlhorithmStarted(int[,] d, int[,] pi) => AlgorithmStarted?.Invoke(this, new Tuple<int[,], int[,]>(d, pi));

        private void OnAlgorithmStepped(int[,] d, int[,] pi) => AlgorithmStepped?.Invoke(this, new Tuple<int[,], int[,]>(d, pi));

        private void OnAlhorithmEnded() => AlgorithmEnded?.Invoke(this, EventArgs.Empty);

        private void OnAlhorithmStopped() => AlgorithmStopped?.Invoke(this, EventArgs.Empty);
    }
}
