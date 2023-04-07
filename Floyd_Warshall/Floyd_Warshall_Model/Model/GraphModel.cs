using Floyd_Warshall_Model.Model.Algorithm;
using Floyd_Warshall_Model.Model.Events;
using Floyd_Warshall_Model.Persistence.Graph;
using Floyd_Warshall_Model.Persistence;

namespace Floyd_Warshall_Model.Model
{
    public class GraphModel
    {
        #region Fields

        private readonly IGraphDataAccess _dataAccess;
        private GraphBase _graph;
        private FloydWarshall? _floydWarshall;
        private int? _prevK;
        private int[,]? _prevPi;
        private int _vertexId;
        private int _edgeId;

        public const int MaxVertexCount = 14;

        #endregion

        #region Properties

        public bool IsDirected { get { return _graph.IsDirected; } }

        public bool IsAlgorithmRunning { get { return _floydWarshall != null && _floydWarshall.IsRunnging; } }

        public bool IsAlgorthmInitialized { get { return _floydWarshall != null; } }

        public int? K { get { return _floydWarshall?.K; } }

        public int? PrevK { get { return _prevK; } }

        #endregion

        #region Events

        public event EventHandler? NewGraphCreated;
        public event EventHandler<GraphLocationEventArgs>? GraphLoaded;
        public event EventHandler<VertexAddedEventArgs>? VertexAdded;
        public event EventHandler<EdgeAddedEventArgs>? DirectedEdgeAdded;
        public event EventHandler<EdgeAddedEventArgs>? UndirectedEdgeAdded;
        public event EventHandler<EdgeUpdatedEventArgs>? EdgeUpdated;
        public event EventHandler? VertexRemoved;
        public event EventHandler<AlgorithmEventArgs>? AlgorithmInitialized;
        public event EventHandler? AlgorithmStepped;
        public event EventHandler? AlgorithmEnded;
        public event EventHandler? AlgorithmCancelled;
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

            _vertexId = 0;
            _edgeId = 0;
            OnNewGraphCreated();
        }

        public void AddVertex()
        {
            if(_graph.VertexCount < MaxVertexCount)
            {
                Vertex v = new Vertex(++_vertexId);
                _graph.AddVertex(v);
                OnVertexAdded(v.Id);
            }
        }

        public void RemoveVertex(int id)
        {
            Vertex? v = _graph.GetVertexById(id);

            if (v != null)
            {
                _graph.RemoveVertex(v);
                OnVertexRemoved();
            }
        }

        public void AddEdge(int fromId, int toId, short weight)
        {
            Vertex? from = _graph.GetVertexById(fromId);
            Vertex? to = _graph.GetVertexById(toId);

            if (from != null && to != null)
            {
                _graph.AddEdge(from, to, weight);
                if (_graph.IsDirected)
                {
                    OnDirectedEdgeAdded(++_edgeId, fromId, toId, weight);
                }
                else
                {
                    OnUndirectedEdgeAdded(++_edgeId, fromId, toId, weight);
                }
            }
        }

        public void RemoveEdge(int fromId, int toId)
        {
            Vertex? from = _graph.GetVertexById(fromId);
            Vertex? to = _graph.GetVertexById(toId);

            if (from != null && to != null)
            {
                _graph.RemoveEdge(from, to);
            }
        }

        public bool IsEdgeBetween(int fromId, int toId)
        {
            Vertex? from = _graph.GetVertexById(fromId);
            Vertex? to = _graph.GetVertexById(toId);

            if (from == null || to == null)
            {
                return false;
            }

            return _graph.GetEdge(from, to) != null; ;
        }

        public short GetWeight(int fromId, int toId)
        {
            Vertex? from = _graph.GetVertexById(fromId);
            Vertex? to = _graph.GetVertexById(toId);

            if (from != null && to != null)
            {
                return _graph.GetWeight(from, to);
            }

            return 0;
        }

        public void UpdateWeight(int fromId, int toId, short weight)
        {
            Vertex? from = _graph.GetVertexById(fromId);
            Vertex? to = _graph.GetVertexById(toId);

            if (from != null && to != null)
            {
                _graph.UpdateWeight(from, to, weight);
                OnEdgeUpdated(fromId, toId);
            }
        }

        public void IncrementWeight(int fromId, int toId)
        {
            Vertex? from = _graph.GetVertexById(fromId);
            Vertex? to = _graph.GetVertexById(toId);

            if (from != null && to != null)
            {
                _graph.IncrementWeight(from, to);
                OnEdgeUpdated(fromId, toId);
            }
        }

        public int GetVertexCount()
        {
            return _graph.VertexCount;
        }

        public List<Edge> GetEdges()
        {
            return _graph.Edges;
        }

        public List<int> GetVertexIds()
        {
            return _graph.VertexIds;
        }

        public async Task LoadAsync(string path)
        {
            if (_dataAccess == null)
            {
                throw new InvalidOperationException("No data acces provided.");
            }

            GraphData v = await _dataAccess.LoadAsync(path);

            if (v.Graph.VertexCount > MaxVertexCount)
            {
                throw new GraphDataException();
            }

            _graph = v.Graph;

            _vertexId = _graph.VertexIds.Max();
            _edgeId = 0;

            OnGraphLoaded(v.VertexDatas.Select(v => new VertexLocation(v.Id, v.X, v.Y)));

            List<Edge> edges = _graph.Edges;

            if (_graph.IsDirected)
            {
                foreach (Edge e in edges)
                {
                    OnDirectedEdgeAdded(++_edgeId, e.From.Id, e.To.Id, e.Weight);
                }
            }
            else
            {
                for (int i = 0; i < edges.Count; ++i)
                {
                    Edge e = edges.Single(e => e.From == edges[i].To && e.To == edges[i].From);
                    edges.Remove(e);
                    OnUndirectedEdgeAdded(++_edgeId, edges[i].From.Id, edges[i].To.Id, e.Weight);
                }
            }
        }

        public async Task SaveAsync(string path, IEnumerable<VertexLocation> locations)
        {
            if (_dataAccess == null)
            {
                throw new InvalidOperationException("No data acces provided.");
            }

            await _dataAccess.SaveAsync(path, _graph, locations.Select(l => new VertexData(l.Id, l.X, l.Y)));
        }

        #endregion

        #region Public algorithm methods

        public void InitAlgorithm()
        {
            _floydWarshall = new FloydWarshall(_graph.ToAdjacencyMatrix(), _graph.VertexIds);
            OnAlgorithmInitialized(_floydWarshall.D, _floydWarshall.Pi);
        }

        public void StepAlgorithm()
        {
            if (_floydWarshall == null) { return; }

            _prevK = _floydWarshall.K;
            _prevPi = _floydWarshall.Pi.Clone() as int[,];

            int res = _floydWarshall.NextStep();

            OnAlgorithmStepped();

            if (res == 0)
            {
                OnAlhorithmEnded();
            }
            else if (res > 0)
            {
                List<int>? route = CreateRoute(res, res, _floydWarshall.Pi);

                if (route != null)
                {
                    OnNegativeCycleFound(route);
                }
            }
        }

        public void CancelAlgorithm()
        {
            _floydWarshall = null;
            _prevK = null;
            _prevPi = null;
            OnAlgorithmCancelled();
        }

        public void GetRoute(int from, int to, bool isPrev)
        {
            List<int>? route = null;

            if (!isPrev && _floydWarshall != null)
            {
                route = CreateRoute(from, to, _floydWarshall.Pi);
            }
            else if (isPrev && _prevPi != null)
            {
                route = CreateRoute(from, to, _prevPi);
            }


            if (route != null)
            {
                OnRouteCreated(route);
            }
        }

        public AlgorithmData? GetAlgorithmData()
        {
            if (_floydWarshall == null)
            {
                return null;
            }

            return new AlgorithmData(_floydWarshall.D, _floydWarshall.Pi, _floydWarshall.ChangesD, _floydWarshall.ChangesPi);
        }

        #endregion

        #region Private methods

        private List<int>? CreateRoute(int from, int to, int[,] pi)
        {
            List<int> route = new List<int>();
            List<int> vertexIds = _graph.VertexIds;

            int fromInd = vertexIds.FindIndex(x => x == from);
            int toInd = vertexIds.FindIndex(x => x == to);

            int next = pi[fromInd, toInd];

            while (next != 0 && next != to)
            {
                route.Add(next);
                int nextInd = vertexIds.FindIndex(x => x == next);
                next = pi[fromInd, nextInd];
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

        private void OnNewGraphCreated() => NewGraphCreated?.Invoke(this, EventArgs.Empty);

        private void OnGraphLoaded(IEnumerable<VertexLocation> locations) => GraphLoaded?.Invoke(this, new GraphLocationEventArgs(locations));

        private void OnVertexAdded(int id) => VertexAdded?.Invoke(this, new VertexAddedEventArgs(id));

        private void OnDirectedEdgeAdded(int id, int from, int to, short weight) => DirectedEdgeAdded?.Invoke(this, new EdgeAddedEventArgs(id, from, to, weight));

        private void OnUndirectedEdgeAdded(int id, int from, int to, short weight) => UndirectedEdgeAdded?.Invoke(this, new EdgeAddedEventArgs(id, from, to, weight));

        private void OnEdgeUpdated(int from, int to) => EdgeUpdated?.Invoke(this, new EdgeUpdatedEventArgs(from, to));

        private void OnVertexRemoved() => VertexRemoved?.Invoke(this, EventArgs.Empty);

        private void OnAlgorithmInitialized(int[,] d, int[,] pi) => AlgorithmInitialized?.Invoke(this, new AlgorithmEventArgs(d, pi));

        private void OnAlgorithmStepped() => AlgorithmStepped?.Invoke(this, EventArgs.Empty);

        private void OnAlhorithmEnded() => AlgorithmEnded?.Invoke(this, EventArgs.Empty);

        private void OnAlgorithmCancelled() => AlgorithmCancelled?.Invoke(this, EventArgs.Empty);

        private void OnNegativeCycleFound(List<int> route) => NegativeCycleFound?.Invoke(this, new RouteEventArgs(route));

        private void OnRouteCreated(List<int> route) => RouteCreated?.Invoke(this, new RouteEventArgs(route));

        #endregion
    }
}
