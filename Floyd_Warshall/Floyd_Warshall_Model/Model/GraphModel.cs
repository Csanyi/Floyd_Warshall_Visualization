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
        private int _vertexId;
        private int _edgeId;

        private int _algorithmPos;
        private int? _k;
        private int? _prevK;
        private int[,]? _d;
        private int[,]? _prevD;
        private int[,]? _pi;
        private int[,]? _prevPi;
        private int? _negCycleInd;
        private List<int>? _negCycle;
        private readonly List<ICollection<ChangeOldNew>> _changesD;
        private readonly List<ICollection<ChangeOldNew>> _changesPi;

        public const int MaxVertexCount = 14;

        #endregion

        #region Properties

        public bool IsDirected { get { return _graph.IsDirected; } }

        public bool IsAlgorthmInitialized { get { return _floydWarshall != null; } }

        public int? K { get { return _k; } }

        public int? PrevK { get { return _prevK; } }

        public bool HasNextStep { get { return _floydWarshall != null && _graph.VertexCount > _algorithmPos && _algorithmPos != _negCycleInd; } }

        public bool HasPreviousStep { get { return _floydWarshall != null && _algorithmPos > 1; } }

        #endregion

        #region Events

        public event EventHandler? NewGraphCreated;
        public event EventHandler<GraphLocationEventArgs>? GraphLoaded;
        public event EventHandler<VertexAddedEventArgs>? VertexAdded;
        public event EventHandler<EdgeAddedEventArgs>? DirectedEdgeAdded;
        public event EventHandler<EdgeAddedEventArgs>? UndirectedEdgeAdded;
        public event EventHandler<EdgeUpdatedEventArgs>? EdgeUpdated;
        public event EventHandler? VertexRemoved;
        public event EventHandler<AlgorithmInitEventArgs>? AlgorithmInitialized;
        public event EventHandler<AlgorithmSteppedEventArgs>? AlgorithmStepped;
        public event EventHandler<AlgorithmSteppedEventArgs>? AlgorithmSteppedBack;
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

            _changesD = new List<ICollection<ChangeOldNew>>();
            _changesPi = new List<ICollection<ChangeOldNew>>();
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
            _algorithmPos = 0;
            _k = 0;
            _d = _floydWarshall.D.Clone() as int[,];
            _pi = _floydWarshall.Pi.Clone() as int[,];
            _prevD = _floydWarshall.D.Clone() as int[,];
            _prevPi = _floydWarshall.Pi.Clone() as int[,];
            OnAlgorithmInitialized(_floydWarshall.D, _floydWarshall.Pi);
        }

        public void StepAlgorithm()
        {
            if (_floydWarshall == null || _d == null || _pi == null || _prevD == null || _prevPi == null) { return; }
            if(!_floydWarshall.IsRunnging && _algorithmPos == _changesD.Count) { return; }

            bool end = false;
            bool neg = false;
            ICollection<ChangeValue> changeD;
            ICollection<ChangeValue> changePi;
            ICollection<ChangeValue> changePrevD;
            ICollection<ChangeValue> changePrevPi;

            ++_algorithmPos;

            if (_algorithmPos - 1 < _changesD.Count && _algorithmPos >= 2)
            {
                _prevK = _k;
                _k = _graph.VertexIds[_algorithmPos-1];

                changeD = UpdateMatrixNew(_d, _changesD[_algorithmPos - 1]);
                changePi = UpdateMatrixNew(_pi, _changesPi[_algorithmPos - 1]);
                changePrevD = UpdateMatrixNew(_prevD, _changesD[_algorithmPos - 2]);
                changePrevPi = UpdateMatrixNew(_prevPi, _changesPi[_algorithmPos - 2]);

                if (_negCycleInd != null && _negCycle != null && _algorithmPos == _negCycleInd)
                {
                    neg = true;
                }
                else if (!HasNextStep)
                {
                    end = true;
                }
            }
            else
            {
                _prevK = _floydWarshall.K;
                changePrevD = UpdateMatrixNew(_prevD, _floydWarshall.ChangesD);
                changePrevPi = UpdateMatrixNew(_prevPi, _floydWarshall.ChangesPi);

                int res = _floydWarshall.NextStep();

                changeD = UpdateMatrixNew(_d, _floydWarshall.ChangesD);
                changePi = UpdateMatrixNew(_pi, _floydWarshall.ChangesPi);
                _k = _floydWarshall.K;

                _changesD.Add(_floydWarshall.ChangesD.ToList());
                _changesPi.Add(_floydWarshall.ChangesPi.ToList());

                if (res == 0)
                {
                    end = true;
                }
                else if (res > 0)
                {
                    List<int>? route = CreateRoute(res, res, _floydWarshall.Pi);

                    if (route != null)
                    {
                        _negCycleInd = _algorithmPos;
                        _negCycle = route;
                        neg = true;
                    }
                }
            }

            OnAlgorithmStepped(changeD, changePi, changePrevD, changePrevPi);

            if (neg && _negCycle != null)
            {
                OnNegativeCycleFound(_negCycle);
            }
            else if (end)
            {
                OnAlgorithmEnded();
            }
        }

        public void StepAlgorithmBack()
        {
            if (_floydWarshall == null || _algorithmPos <= 1 || _d == null || _pi == null || _prevD == null || _prevPi == null) { return; }
            
            --_algorithmPos;

            _k = _prevK;
            _prevK = _algorithmPos == 1 ? 0 : _graph.VertexIds[_algorithmPos - 2];

            ICollection<ChangeValue> changeD = UpdateMatrixOld(_d, _changesD[_algorithmPos]);
            ICollection<ChangeValue> changePi = UpdateMatrixOld(_pi, _changesPi[_algorithmPos]);
            ICollection<ChangeValue> changePrevD = UpdateMatrixOld(_prevD, _changesD[_algorithmPos - 1]);
            ICollection<ChangeValue> changePrevPi = UpdateMatrixOld(_prevPi, _changesPi[_algorithmPos - 1]);

            OnAlgorithmSteppedBack(changeD, changePi, changePrevD, changePrevPi);
        }

        public void CancelAlgorithm()
        {
            _floydWarshall = null;
            _algorithmPos = 0;
            _negCycle = null;
            _negCycleInd = null;
            _k = null;
            _prevK = null;
            _d = null;
            _pi = null;
            _prevD = null;
            _prevPi = null;
            _changesD.Clear();
            _changesPi.Clear();
            OnAlgorithmCancelled();
        }

        public void GetRoute(int from, int to, bool isPrev)
        {
            List<int>? route = null;

            if (!isPrev && _pi != null)
            {
                route = CreateRoute(from, to, _pi);
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
            if (_floydWarshall == null ||_d == null || _pi == null || _prevD == null ||_prevPi == null)
            {
                return null;
            }

            return new AlgorithmData(_d, _pi, _prevD, _prevPi,
                _changesD[_algorithmPos-1].Select(c => c as Change).ToHashSet(),
                _changesPi[_algorithmPos-1].Select(c => c as Change).ToHashSet());
        }

        #endregion

        #region Private methods

        private ICollection<ChangeValue> UpdateMatrixNew(int[,] matrix, ICollection<ChangeOldNew> changes)
        {
            ICollection<ChangeValue> result = new HashSet<ChangeValue>();

            foreach (ChangeOldNew c in changes)
            {
                matrix[c.I, c.J] = c.NewValue;
                result.Add(new ChangeValue(c.I, c.J, c.NewValue));
            }

            return result;
        }

        private ICollection<ChangeValue> UpdateMatrixOld(int[,] matrix, ICollection<ChangeOldNew> changes)
        {
            ICollection<ChangeValue> result = new HashSet<ChangeValue>();

            foreach (ChangeOldNew c in changes)
            {
                matrix[c.I, c.J] = c.OldValue;
                result.Add(new ChangeValue(c.I, c.J, c.OldValue));
            }

            return result;
        }

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

        private void OnAlgorithmInitialized(int[,] d, int[,] pi) => AlgorithmInitialized?.Invoke(this, new AlgorithmInitEventArgs(d, pi));

        private void OnAlgorithmStepped(ICollection<ChangeValue> d, ICollection<ChangeValue> pi,
            ICollection<ChangeValue> prevD, ICollection<ChangeValue> prevPi) 
            => AlgorithmStepped?.Invoke(this, new AlgorithmSteppedEventArgs(d, pi, prevD, prevPi));

        private void OnAlgorithmSteppedBack(ICollection<ChangeValue> d, ICollection<ChangeValue> pi,
          ICollection<ChangeValue> prevD, ICollection<ChangeValue> prevPi)
          => AlgorithmSteppedBack?.Invoke(this, new AlgorithmSteppedEventArgs(d, pi, prevD, prevPi));

        private void OnAlgorithmEnded() => AlgorithmEnded?.Invoke(this, EventArgs.Empty);

        private void OnAlgorithmCancelled() => AlgorithmCancelled?.Invoke(this, EventArgs.Empty);

        private void OnNegativeCycleFound(List<int> route) => NegativeCycleFound?.Invoke(this, new RouteEventArgs(route));

        private void OnRouteCreated(List<int> route) => RouteCreated?.Invoke(this, new RouteEventArgs(route));

        #endregion
    }
}
