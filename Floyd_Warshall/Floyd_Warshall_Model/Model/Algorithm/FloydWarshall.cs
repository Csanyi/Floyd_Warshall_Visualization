namespace Floyd_Warshall_Model.Model.Algorithm
{
    /// <summary>
    /// Type of the Floyd-Warshall algorithm
    /// </summary>
    public class FloydWarshall
    {
        #region Fields

        private readonly int[,] _graph;                        // adjacency matrix representation of the graph
        private readonly List<int> _vertexIds;                 // vertex ids of the graph

        private readonly int[,] _d;                            // the D matrix
        private readonly int[,] _pi;                           // the Pi matrix
        private readonly ICollection<ChangeOldNew> _changesD;  // changes of the D matrix
        private readonly ICollection<ChangeOldNew> _changesPi; // changes of the Pi matrix

        private int _k;                                        // vertex id being processed
        private bool _isRunning;                               // algorithm state

        #endregion

        #region Properties

        /// <summary>
        /// Gets the D matrix
        /// </summary>
        public int[,] D { get { return _d; } }

        /// <summary>
        /// Gets the Pi matrix
        /// </summary>
        public int[,] Pi { get { return _pi; } }

        /// <summary>
        /// Gets the changes of the D martrix
        /// </summary>
        public ICollection<ChangeOldNew> ChangesD { get { return _changesD; } }

        /// <summary>
        /// Gets the changes of the Pi matrix
        /// </summary>
        public ICollection<ChangeOldNew> ChangesPi { get { return _changesPi; } }

        /// <summary>
        /// Gets the vertex id under processing
        /// </summary>
        public int K { get { return _k == 0 ? 0 : _vertexIds[_k - 1]; } }

        /// <summary>
        /// Gets the algorithm state
        /// </summary>
        public bool IsRunnging { get { return _isRunning; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Consturctor of the Floyd-Warshall algorithm
        /// </summary>
        /// <param name="graph">The adjacency matrix representation of the graph</param>
        /// <param name="vertexIds">The vertex ids of the graph</param>
        public FloydWarshall(int[,] graph, List<int> vertexIds)
        {
            _graph = graph;
            _vertexIds = vertexIds;
            _d = new int[graph.GetLength(0), graph.GetLength(1)];
            _pi = new int[graph.GetLength(0), graph.GetLength(1)];
            _changesD = new HashSet<ChangeOldNew>();
            _changesPi = new HashSet<ChangeOldNew>();
            _k = 0;
            _isRunning = true;

            Initaliaze();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Initializes the algorithm
        /// </summary>
        private void Initaliaze()
        {
            for (int i = 0; i < _graph.GetLength(0); ++i)
            {
                for (int j = 0; j < _graph.GetLength(1); ++j)
                {
                    _d[i, j] = _graph[i, j];

                    if (i != j && _graph[i, j] < int.MaxValue)
                    {
                        _pi[i, j] = _vertexIds[i];
                    }
                    else
                    {
                        _pi[i, j] = 0;
                    }
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Steps the algorithm
        /// </summary>
        /// <returns>0 if the algorithm is finished, >0 if negative cycle is found, otherwise -1</returns>
        public int NextStep()
        {
            if (!_isRunning || _k >= _graph.GetLength(0))
            {
                _isRunning = false;
                return 0;
            }

            _changesD.Clear();
            _changesPi.Clear();

            for (int i = 0; i < _graph.GetLength(0); ++i)
            {
                for (int j = 0; j < _graph.GetLength(1); ++j)
                {
                    if (_d[i, _k] != int.MaxValue && _d[_k, j] != int.MaxValue && _d[i, j] > _d[i, _k] + _d[_k, j])
                    {
                        _changesD.Add(new ChangeOldNew(i, j, _d[i, j], _d[i, _k] + _d[_k, j]));
                        _d[i, j] = _d[i, _k] + _d[_k, j];
                       
                        if (_pi[i, j] != _pi[_k, j])
                        {
                            _changesPi.Add(new ChangeOldNew(i, j, _pi[i, j], _pi[_k, j]));
                            _pi[i, j] = _pi[_k, j];
                        }

                        if (i == j && _d[i, i] < 0)
                        {
                            ++_k;
                            _isRunning = false;
                            return _vertexIds[i];
                        }
                    }
                }
            }

            if (++_k >= _graph.GetLength(0))
            {
                _isRunning = false;
                return 0;
            }

            return -1;
        }

        #endregion
    }
}
