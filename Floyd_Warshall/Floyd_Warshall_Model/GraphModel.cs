using Floyd_Warshall_Model.Persistence;
using Floyd_Warshall_Model.Graph;
using Floyd_Warshall_Model.Events;

namespace Floyd_Warshall_Model
{
    public class GraphModel
    {
        #region Fields

        private GraphBase _graph;
        private FloydWarshall? _floydWarshall;
        private readonly IGraphDataAccess _dataAccess;

        #endregion

        #region Properties

        public bool IsDirected { get { return _graph.IsDirected; } }

        public bool IsAlgorithmRunning { get { return _floydWarshall != null && _floydWarshall.IsRunnging; } }

        public bool IsAlgorthmInitialized { get { return _floydWarshall != null; } }

        public int? K { get { return _floydWarshall?.K; } }

        #endregion

        #region Events

        public event EventHandler? NewEmptyGraph;
        public event EventHandler<GraphLoadedEventArgs>? GraphLoaded;
        public event EventHandler? VertexAdded;
        public event EventHandler? VertexRemoved;
        public event EventHandler<AlgorithmEventArgs>? AlgorithmStarted;
        public event EventHandler? AlgorithmStepped;
        public event EventHandler? AlgorithmEnded;
        public event EventHandler? AlgorithmStopped;
        public event EventHandler<RouteEventArgs>? NegativeCycleFound;
        public event EventHandler<RouteEventArgs>? RouteCreated;

        #endregion

        #region Constructors

        public GraphModel(IGraphDataAccess dataAccess)
        {
            _graph = new UndirectedGraph();
            _dataAccess = dataAccess;
        }

        #endregion

        #region Public graph methods

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

            GraphData v = await _dataAccess.LoadAsync(path);

            _graph = v.Graph;

            OnGraphLoaded(v.VertexLocations);
        }

        public async Task SaveAsync(string path, IEnumerable<VertexLocation> locations)
        {
            if (_dataAccess == null)
            {
                throw new InvalidOperationException("No data acces provided.");
            }

            await _dataAccess.SaveAsync(path, _graph, locations);
        }

        #endregion

        #region Public algorithm methods

        public void StartAlgorithm()
        {
            _floydWarshall = new FloydWarshall(_graph.ToAdjacencyMatrix(), _graph.GetVertexIds() );
            OnAlhorithmStarted(_floydWarshall.D, _floydWarshall.Pi);
        }

        public void StepAlgorithm()
        {
            if( _floydWarshall == null ) { return; }

            int res = _floydWarshall.NextStep();

            OnAlgorithmStepped(_floydWarshall.D, _floydWarshall.Pi);

            if(res == 0)
            {
                OnAlhorithmEnded();
            }
            else if(res > 0)
            {
                List<int>? route = CreateRoute(res, res);

                if(route != null)
                {
                    OnNegativeCycleFound(route);
                }
            }
        }

        public void StopAlgorithm()
        {
            _floydWarshall = null;
            OnAlhorithmStopped();
        }

        public void GetRoute(int from, int to)
        {
           List<int>? route = CreateRoute(from, to);

            if(route != null)
            {
                OnRouteCreated(route);
            }
        }

        public AlgorithmData? GetAlgorithmData()
        {
            if(_floydWarshall == null)
            {
                return null;
            }

            return new AlgorithmData(_floydWarshall.D, _floydWarshall.Pi);
        }

        #endregion

        #region Private methods

        private List<int>? CreateRoute(int from, int to)
        {
            if (_floydWarshall == null)
            {
                return null;
            }

            List<int> route = new List<int>();
            List<int> vertexIds = _graph.GetVertexIds();

            int fromInd = vertexIds.FindIndex(x => x == from);
            int toInd = vertexIds.FindIndex(x => x == to);

            int next = _floydWarshall.Pi[fromInd, toInd];

            while (next != 0 && next != to)
            {
                route.Add(next);
                int nextInd = vertexIds.FindIndex(x => x == next);
                next = _floydWarshall.Pi[fromInd, nextInd];
            }

            route.Reverse();
            if (from == to || route.Any())
            {
                route.Add(to);
            }

            return route;
        }

        #endregion

        #region Private event methods

        private void OnNewEmptyGraph() => NewEmptyGraph?.Invoke(this, EventArgs.Empty);

        private void OnGraphLoaded(IEnumerable<VertexLocation> locations) => GraphLoaded?.Invoke(this, new GraphLoadedEventArgs(locations));

        private void OnVertexAdded() => VertexAdded?.Invoke(this, EventArgs.Empty);

        private void OnVertexRemoved() => VertexRemoved?.Invoke(this, EventArgs.Empty);

        private void OnAlhorithmStarted(int[,] d, int[,] pi) => AlgorithmStarted?.Invoke(this, new AlgorithmEventArgs(d, pi));

        private void OnAlgorithmStepped(int[,] d, int[,] pi) => AlgorithmStepped?.Invoke(this, EventArgs.Empty);

        private void OnAlhorithmEnded() => AlgorithmEnded?.Invoke(this, EventArgs.Empty);

        private void OnAlhorithmStopped() => AlgorithmStopped?.Invoke(this, EventArgs.Empty);

        private void OnNegativeCycleFound(List<int> route) => NegativeCycleFound?.Invoke(this, new RouteEventArgs(route));

        private void OnRouteCreated(List<int> route) => RouteCreated?.Invoke(this, new RouteEventArgs(route));

        #endregion
    }
}
