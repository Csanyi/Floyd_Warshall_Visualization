namespace Floyd_Warshall_Model.Model.Algorithm
{
    public class FloydWarshall
    {
        private readonly int[,] _graph;
        private readonly List<int> _vertexIds;

        private readonly int[,] _d;
        public int[,] D { get { return _d; } }

        private readonly int[,] _pi;
        public int[,] Pi { get { return _pi; } }

        private readonly ICollection<Tuple<int, int>> _changesD;
        public ICollection<Tuple<int, int>> ChangesD { get { return _changesD; } }

        private readonly ICollection<Tuple<int, int>> _changesPi;
        public ICollection<Tuple<int, int>> ChangesPi { get { return _changesPi; } }

        private int _k;
        public int K { get { return _k == 0 ? 0 : _vertexIds[_k - 1]; } }

        private bool _isRunning;
        public bool IsRunnging { get { return _isRunning; } }

        public FloydWarshall(int[,] graph, List<int> vertexIds)
        {
            _graph = graph;
            _vertexIds = vertexIds;
            _d = new int[graph.GetLength(0), graph.GetLength(1)];
            _pi = new int[graph.GetLength(0), graph.GetLength(1)];
            _changesD = new HashSet<Tuple<int, int>>();
            _changesPi = new HashSet<Tuple<int, int>>();
            _k = 0;
            _isRunning = true;

            Initaliaze();
        }

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
                        Tuple<int, int> change = new Tuple<int, int>(i, j);
                        _changesD.Add(change);
                        if (_pi[i, j] != _pi[_k, j])
                        {
                            _changesPi.Add(change);
                        }

                        _d[i, j] = _d[i, _k] + _d[_k, j];
                        _pi[i, j] = _pi[_k, j];

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
    }
}
