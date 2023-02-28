namespace Floyd_Warshall_Model
{
    public class Floyd_Warshall
    {
        private int[,] _graph;

        private int[,] _d;
        public int[,] D { get; }

        private int[,] _pi;
        public int[,] Pi { get; }

        private int _i = 0;
        private int _j = 0;
        private int _k = 0;

        private bool _isRunning = true;
        public bool IsRunnging { get; }

        public Floyd_Warshall(int[,] graph) 
        {
            _graph = graph;
            _d = new int[graph.GetLength(0), graph.GetLength(1)];
            _pi = new int[graph.GetLength(0), graph.GetLength(1)];

            Initaliaze();
        }

        private void Initaliaze()
        {
            for(int i = 0; i < _graph.GetLength(0); ++i)
            {
                for(int j = 0; j < _graph.GetLength(1); ++j)
                {
                    _d[i, j] = _graph[i, j];

                    if(i != j && _graph[i,j] < int.MaxValue)
                    {
                        _pi[i, j] = i;
                    }
                    else
                    {
                        _pi[i, j] = 0;
                    }
                }
            }
        }

        public bool NextStep()
        {
            if(!_isRunning) { return false; }

            if (_d[_i, _j] > _d[_i, _k] + _d[_k, _j])
            {
                _d[_i, _j] = _d[_i, _k] + _d[_k, _j];
                _pi[_i, _j] = _pi[_k, _j];
                if(_i == _j && _d[_i, _i] < 0)
                {
                    _isRunning = false;
                    return false;
                }
            }

            if(++_j >= _graph.GetLength(1))
            {
                _j = 0;
                if(++_i >= _graph.GetLength(0))
                {
                    _i = 0;
                    if(++_k >= _graph.GetLength(0))
                    {
                        _isRunning = false;
                        return true;
                    }
                }
            }

            return true;
        }
    }
}
