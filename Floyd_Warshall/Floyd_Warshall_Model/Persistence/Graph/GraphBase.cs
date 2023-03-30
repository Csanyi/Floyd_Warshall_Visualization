namespace Floyd_Warshall_Model.Persistence.Graph
{
    public abstract class GraphBase
    {
        #region Fields

        protected const short MaxValue = 9999;
        protected const short MinValue = -MaxValue;

        protected IDictionary<Vertex, ICollection<Edge>> _adjacenylist;

        #endregion

        #region Constructors

        protected GraphBase()
        {
            _adjacenylist = new Dictionary<Vertex, ICollection<Edge>>();
        }

        #endregion

        #region Properties

        public abstract bool IsDirected { get; }

        public List<Vertex> Vertices
        {
            get { return _adjacenylist.Keys.OrderBy(v => v.Id).ToList(); }
        }

        public List<int> VertexIds
        {
            get { return _adjacenylist.Keys.Select(v => v.Id).OrderBy(i => i).ToList(); }
        }

        public List<Edge> Edges
        {
            get
            {
                List<Edge> edges = new List<Edge>();

                foreach (var v in _adjacenylist)
                {
                    edges.AddRange(v.Value);
                }

                return edges;
            }
        }

        public int EdgeCount
        {
            get { return _adjacenylist.Sum(x => x.Value.Count); }
        }

        public int VertexCount
        {
            get { return _adjacenylist.Count; }
        }

        #endregion

        #region Methods

        public abstract void AddEdge(Vertex from, Vertex to, short weight);

        public abstract void RemoveEdge(Vertex from, Vertex to);

        public abstract void UpdateWeight(Vertex from, Vertex to, short weight);

        public abstract void IncrementWeight(Vertex from, Vertex to);

        public void AddVertex(Vertex v)
        {
            Check(() => !_adjacenylist.ContainsKey(v));

            _adjacenylist.Add(v, new List<Edge>());
        }

        public void RemoveVertex(Vertex v)
        {
            Check(() => _adjacenylist.ContainsKey(v));

            foreach (var adjacent in _adjacenylist)
            {
                Edge? e = GetEdge(adjacent.Key, v);

                if (e != null)
                {
                    adjacent.Value.Remove(e);
                }
            }

            _adjacenylist.Remove(v);
        }

        public Edge? GetEdge(Vertex from, Vertex to)
        {
            Check(() => _adjacenylist.ContainsKey(from));

            return _adjacenylist[from].FirstOrDefault(e => e.To == to);
        }

        public Vertex? GetVertexById(int id) 
        { 
            return _adjacenylist.Keys.FirstOrDefault(v => v.Id == id);
        }

        public short GetWeight(Vertex from, Vertex to)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));

            return _adjacenylist[from].First(e => e.To == to).Weight;
        }

        public int[,] ToAdjacencyMatrix()
        {
            int size = VertexCount;
            IList<Vertex> vertices = Vertices;

            int[,] adjacencyMatrix = new int[size, size];

            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    Edge? e = GetEdge(vertices[i], vertices[j]);

                    adjacencyMatrix[i, j] = e != null ? e.Weight : int.MaxValue;
                }

                adjacencyMatrix[i, i] = 0;
            }

            return adjacencyMatrix;
        }

        protected static void Check(Func<bool> cond)
        {
            if (!cond())
            {
                throw new InvalidOperationException();
            }
        }

        #endregion
    }
}
