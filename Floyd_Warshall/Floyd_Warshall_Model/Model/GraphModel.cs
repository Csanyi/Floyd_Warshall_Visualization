using Floyd_Warshall_Model.Model.Algorithm;
using Floyd_Warshall_Model.Model.Events;
using Floyd_Warshall_Model.Persistence.Graph;
using Floyd_Warshall_Model.Persistence;

namespace Floyd_Warshall_Model.Model
{
    /// <summary>
    /// Type of the graph model
    /// </summary>
    public class GraphModel
    {
        #region Fields

        private readonly IGraphDataAccess _dataAccess;               // data access
        private GraphBase _graph;                                    // graph
        private FloydWarshall? _floydWarshall;                       // Floyd-Warshall algorithm
        private int _vertexId;                                       // the id of the next vertex
        private int _edgeId;                                         // the id of the next edge

        private int _algorithmPos;                                   // current algorithm position
        private int? _k;                                             // vertex id being processed
        private int? _prevK;                                         // previous vertex id being processed
        private int[,]? _d;                                          // the D matrix
        private int[,]? _prevD;                                      // the previous D matrix
        private int[,]? _pi;                                         // the Pi matrix
        private int[,]? _prevPi;                                     // the previous Pi matrix
        private int? _negCycleInd;                                   // the step when the algoithm found the negative cycle
        private List<int>? _negCycle;                                // the negative cycle
        private readonly List<ICollection<ChangeOldNew>> _changesD;  // collection of changes of the D matrix
        private readonly List<ICollection<ChangeOldNew>> _changesPi; // collection of changes of the Pi matrix

        public const int MaxVertexCount = 14;                        // the maximum number of verteces

        #endregion

        #region Properties

        /// <summary>
        /// Gets the graph direction
        /// </summary>
        public bool IsDirected { get { return _graph.IsDirected; } }

        /// <summary>
        /// Indicates whether the algorithm is initialized
        /// </summary>
        public bool IsAlgorthmInitialized { get { return _floydWarshall != null; } }

        /// <summary>
        /// Gets vertex id being processed
        /// </summary>
        public int? K { get { return _k; } }

        /// <summary>
        /// Gets previous vertex id being processed
        /// </summary>
        public int? PrevK { get { return _prevK; } }

        /// <summary>
        /// Indicate whether the algorithm has next step
        /// </summary>
        public bool HasNextStep { get { return _floydWarshall != null && _graph.VertexCount > _algorithmPos && _algorithmPos != _negCycleInd; } }

        /// <summary>
        /// Indicates whether the algorithm has previous step
        /// </summary>
        public bool HasPreviousStep { get { return _floydWarshall != null && _algorithmPos > 1; } }

        #endregion

        #region Events

        public event EventHandler<NewGraphEventArgs>? NewGraphCreated;
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

        /// <summary>
        /// Constuctor of the graph model
        /// </summary>
        /// <param name="dataAccess">The data access</param>
        public GraphModel(IGraphDataAccess dataAccess)
        {
            _graph = new UndirectedGraph();
            _dataAccess = dataAccess;

            _changesD = new List<ICollection<ChangeOldNew>>();
            _changesPi = new List<ICollection<ChangeOldNew>>();
        }

        #endregion

        #region Public graph methods

        /// <summary>
        /// Creates a new graph
        /// </summary>
        /// <param name="isDirected">The direction of the new graph</param>
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
            OnNewGraphCreated(isDirected);
        }

        /// <summary>
        /// Adds a new vertex to the graph
        /// </summary>
        public void AddVertex()
        {
            if(_graph.VertexCount < MaxVertexCount)
            {
                Vertex v = new Vertex(++_vertexId);
                _graph.AddVertex(v);
                OnVertexAdded(v.Id);
            }
        }

        /// <summary>
        /// Removes the specified vertex from the graph
        /// </summary>
        /// <param name="id">Id of the vertex to remove</param>
        public void RemoveVertex(int id)
        {
            Vertex? v = _graph.GetVertexById(id);

            if (v != null)
            {
                _graph.RemoveVertex(v);
                OnVertexRemoved();
            }
        }

        /// <summary>
        /// Adds a new edge to the graph
        /// </summary>
        /// <param name="fromId">The id of the start vertex</param>
        /// <param name="toId">The id of the end vertex</param>
        /// <param name="weight">The weight of the edge</param>
        public void AddEdge(int fromId, int toId, short weight)
        {
            Vertex? from = _graph.GetVertexById(fromId);
            Vertex? to = _graph.GetVertexById(toId);

            if (from != null && to != null && from != to && _graph.GetEdge(from, to) == null)
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

        /// <summary>
        /// Removes the specified edge form the graph
        /// </summary>
        /// <param name="fromId">The id of the start vertex</param>
        /// <param name="toId">The id of the end vertex</param>
        public void RemoveEdge(int fromId, int toId)
        {
            Vertex? from = _graph.GetVertexById(fromId);
            Vertex? to = _graph.GetVertexById(toId);

            if (from != null && to != null)
            {
                _graph.RemoveEdge(from, to);
            }
        }

        /// <summary>
        /// Indicates whether an edge exist between the two specifien vertex
        /// </summary>
        /// <param name="fromId"></param>
        /// <param name="toId"></param>
        /// <returns>true, if the edge exists, otherwise false</returns>
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

        /// <param name="fromId">The start vertex id</param>
        /// <param name="toId">The end vertex id</param>
        /// <returns>The weight of the specified edge</returns>
        public short? GetWeight(int fromId, int toId)
        {
            Vertex? from = _graph.GetVertexById(fromId);
            Vertex? to = _graph.GetVertexById(toId);

            if (from != null && to != null)
            {
                return _graph.GetWeight(from, to);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Updates the specified edge weight to the specified value
        /// </summary>
        /// <param name="fromId">The start vertex id</param>
        /// <param name="toId">The end vertex id</param>
        /// <param name="weight">The new edge weight</param>
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

        /// <summary>
        /// Increases by one the specified edge weight
        /// </summary>
        /// <param name="fromId">The start vertex id</param>
        /// <param name="toId">The end edge id</param>
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

        /// <returns>The number of verteces of the graph</returns>
        public int GetVertexCount()
        {
            return _graph.VertexCount;
        }

        /// <returns>The number of edges of the graph</returns>
        public int GetEdgeCount()
        {
            return _graph.EdgeCount;
        }

        /// <returns>The vertex ids of the graph in ascending order</returns>
        public List<int> GetVertexIds()
        {
            return _graph.VertexIds;
        }

        /// <summary>
        /// Loads graph
        /// </summary>
        /// <param name="path">File path</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="GraphDataException"></exception>
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

            OnGraphLoaded(v.VertexData.Select(v => new VertexLocation(v.Id, v.X, v.Y)));

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

        /// <summary>
        /// Saves graph
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="locations">Locations of the verteces</param>
        /// <exception cref="InvalidOperationException"></exception>
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

        /// <summary>
        /// Initializes the algorithm
        /// </summary>
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

        /// <summary>
        /// Steps forward the algorithm
        /// </summary>
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

        /// <summary>
        /// Steps backward the algorithm
        /// </summary>
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

        /// <summary>
        /// Cancels the algorithm
        /// </summary>
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

        /// <summary>
        /// Creates the route between the specified verteces
        /// </summary>
        /// <param name="from">The start vertex id</param>
        /// <param name="to">The end vertex id</param>
        /// <param name="isPrev">Computes previous step route if true</param>
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

        /// <returns>The current algorithm data</returns>
        public AlgorithmData? GetAlgorithmData()
        {
            if (_floydWarshall == null ||_d == null || _pi == null || _prevD == null || _prevPi == null)
            {
                return null;
            }

            if(_algorithmPos < 1)
            {
                return new AlgorithmData(_d, _pi, _prevD, _prevPi,
                        new HashSet<Change>(),
                        new HashSet<Change>());
            }
            else
            {
                return new AlgorithmData(_d, _pi, _prevD, _prevPi,
                        _changesD[_algorithmPos - 1].Select(c => c as Change).ToHashSet(),
                        _changesPi[_algorithmPos - 1].Select(c => c as Change).ToHashSet());
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Updates the specified matrix with the new values
        /// </summary>
        /// <param name="matrix">Matrix to update</param>
        /// <param name="changes">Changes for update</param>
        /// <returns>Collection of changes, that only contains the new values</returns>
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

        /// <summary>
        /// Updates the specified matrix with the old values
        /// </summary>
        /// <param name="matrix">Matrix to update</param>
        /// <param name="changes">Changes for update</param>
        /// <returns>Collection of changes, that only contains the old values</returns>
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

        /// <summary>
        /// Creates the route between the specified vertex ids, using the specified matrix
        /// </summary>
        /// <param name="from">The start vertex id</param>
        /// <param name="to">The end vertex id</param>
        /// <param name="pi">Matrix to compute the route</param>
        /// <returns>The route</returns>
        private List<int>? CreateRoute(int from, int to, int[,] pi)
        {
            List<int> route = new List<int>();
            List<int> vertexIds = _graph.VertexIds;

            int fromInd = vertexIds.FindIndex(x => x == from);
            int toInd = vertexIds.FindIndex(x => x == to);

            if(fromInd == -1 || toInd == -1) { return null; }

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

        /// <summary>
        /// Triggers the NewGraphCreated event
        /// </summary>
        /// <param name="isDirected">The direction of the new graph</param>
        private void OnNewGraphCreated(bool isDirected) => NewGraphCreated?.Invoke(this, new NewGraphEventArgs(isDirected));

        /// <summary>
        /// Triggers the GraphLoaded event
        /// </summary>
        /// <param name="locations">The locations of the verteces</param>
        private void OnGraphLoaded(IEnumerable<VertexLocation> locations) => GraphLoaded?.Invoke(this, new GraphLocationEventArgs(locations));

        /// <summary>
        /// Triggers the VertexAdded event
        /// </summary>
        /// <param name="id">The id of the added vertex</param>
        private void OnVertexAdded(int id) => VertexAdded?.Invoke(this, new VertexAddedEventArgs(id));

        /// <summary>
        /// Triggers the DirectedEdgeAdded event
        /// </summary>
        /// <param name="id">The id of the added edge</param>
        /// <param name="from">The start vertex id</param>
        /// <param name="to">The end vertex id</param>
        /// <param name="weight">The edge weight</param>
        private void OnDirectedEdgeAdded(int id, int from, int to, short weight) => DirectedEdgeAdded?.Invoke(this, new EdgeAddedEventArgs(id, from, to, weight));

        /// <summary>
        /// Triggers the UndirectedEdgeAdded event
        /// </summary>
        /// <param name="id">The id of the added edge</param>
        /// <param name="from">The start vertex id</param>
        /// <param name="to">The end vertex id</param>
        /// <param name="weight">The edge weight</param>
        private void OnUndirectedEdgeAdded(int id, int from, int to, short weight) => UndirectedEdgeAdded?.Invoke(this, new EdgeAddedEventArgs(id, from, to, weight));

        /// <summary>
        /// Triggers the EdgeUpdated event
        /// </summary>
        /// <param name="from">The start vertex id of the updated edge</param>
        /// <param name="to">The end vertex id of the updated edge</param>
        private void OnEdgeUpdated(int from, int to) => EdgeUpdated?.Invoke(this, new EdgeUpdatedEventArgs(from, to));

        /// <summary>
        /// Triggers the VertexRemoved event
        /// </summary>
        private void OnVertexRemoved() => VertexRemoved?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Triggers the AlgorithmInitialized event
        /// </summary>
        /// <param name="d">The D matrix</param>
        /// <param name="pi">The Pi matrix</param>
        private void OnAlgorithmInitialized(int[,] d, int[,] pi) => AlgorithmInitialized?.Invoke(this, new AlgorithmInitEventArgs(d, pi));

        /// <summary>
        /// Triggers the OnAlgorithmStepped event
        /// </summary>
        /// <param name="d">Chages of the D matrix</param>
        /// <param name="pi">Chages of the Pi matrix</param>
        /// <param name="prevD">Changes of the previous D matrix</param>
        /// <param name="prevPi">Changes of the previous Pi matrix</param>
        private void OnAlgorithmStepped(ICollection<ChangeValue> d, ICollection<ChangeValue> pi,
            ICollection<ChangeValue> prevD, ICollection<ChangeValue> prevPi) 
            => AlgorithmStepped?.Invoke(this, new AlgorithmSteppedEventArgs(d, pi, prevD, prevPi));

        /// <summary>
        /// Triggers the OnAlgorithmSteppedBack event
        /// </summary>
        /// <param name="d">Chages of the D matrix</param>
        /// <param name="pi">Chages of the Pi matrix</param>
        /// <param name="prevD">Changes of the previous D matrix</param>
        /// <param name="prevPi">Changes of the previous Pi matrix</param>
        private void OnAlgorithmSteppedBack(ICollection<ChangeValue> d, ICollection<ChangeValue> pi,
          ICollection<ChangeValue> prevD, ICollection<ChangeValue> prevPi)
          => AlgorithmSteppedBack?.Invoke(this, new AlgorithmSteppedEventArgs(d, pi, prevD, prevPi));

        /// <summary>
        /// Triggers the AlgorithmEnded event
        /// </summary>
        private void OnAlgorithmEnded() => AlgorithmEnded?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Triggers the AlgorithmCancelled event
        /// </summary>
        private void OnAlgorithmCancelled() => AlgorithmCancelled?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Triggers the NegativeCycleFound event
        /// </summary>
        /// <param name="route">The negative cycle</param>
        private void OnNegativeCycleFound(List<int> route) => NegativeCycleFound?.Invoke(this, new RouteEventArgs(route));

        /// <summary>
        /// Triggers the RouteCreated event
        /// </summary>
        /// <param name="route">The route</param>
        private void OnRouteCreated(List<int> route) => RouteCreated?.Invoke(this, new RouteEventArgs(route));

        #endregion
    }
}
