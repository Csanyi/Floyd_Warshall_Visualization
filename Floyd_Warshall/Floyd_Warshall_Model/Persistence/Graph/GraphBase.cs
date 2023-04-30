namespace Floyd_Warshall_Model.Persistence.Graph
{
    /// <summary>
    /// Base type of the graph
    /// </summary>
    public abstract class GraphBase
    {
        #region Fields

        protected const short MaxValue = 9999;                          // maximum edge weight
        protected const short MinValue = -MaxValue;                     // minimum edge weight

        protected IDictionary<Vertex, ICollection<Edge>> _adjacenylist; // the adjacecny list representation of the graph

        #endregion

        #region Constructors

        /// <summary>
        /// The constructor of the GraphBase
        /// </summary>
        protected GraphBase()
        {
            _adjacenylist = new Dictionary<Vertex, ICollection<Edge>>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the direction of the graph
        /// </summary>
        public abstract bool IsDirected { get; }

        /// <summary>
        /// Gets the verteces of the graph
        /// </summary>
        public List<Vertex> Vertices
        {
            get { return _adjacenylist.Keys.OrderBy(v => v.Id).ToList(); }
        }

        /// <summary>
        /// Gets the vertex ids of the graph in ascending order
        /// </summary>
        public List<int> VertexIds
        {
            get { return _adjacenylist.Keys.Select(v => v.Id).OrderBy(i => i).ToList(); }
        }

        /// <summary>
        /// Gets the edges of the graph
        /// </summary>
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

        /// <summary>
        /// Gets the number of edges of the graph
        /// </summary>
        public int EdgeCount
        {
            get { return _adjacenylist.Sum(x => x.Value.Count); }
        }

        /// <summary>
        /// Gets the number of verteces of the graph
        /// </summary>
        public int VertexCount
        {
            get { return _adjacenylist.Count; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Adds an an edge to the graph
        /// </summary>
        /// <param name="from">The start vertex of the edge</param>
        /// <param name="to">The end vertex of the edge</param>
        /// <param name="weight">The weight of the edge</param>
        public abstract void AddEdge(Vertex from, Vertex to, short weight);

        /// <summary>
        /// Removes the specified edge from the graph
        /// </summary>
        /// <param name="from">The start vertex of the edge</param>
        /// <param name="to">The end vertex of the edge</param>
        public abstract void RemoveEdge(Vertex from, Vertex to);

        /// <summary>
        /// Updates the weight of the specified edge to the specified  value
        /// </summary>
        /// <param name="from">The start vertex of the edge</param>
        /// <param name="to">The end vertex of the edge</param>
        /// <param name="weight">The new weight</param>
        public abstract void UpdateWeight(Vertex from, Vertex to, short weight);

        /// <summary>
        /// Increases by one the weight of the specified edge
        /// </summary>
        /// <param name="from">The start vertex of the edge</param>
        /// <param name="to">The end vertex of the edge</param>
        public abstract void IncrementWeight(Vertex from, Vertex to);

        /// <summary>
        /// Adds a vertex to the graph
        /// </summary>
        /// <param name="v">The vertex to add</param>
        public void AddVertex(Vertex v)
        {
            Check(() => !_adjacenylist.ContainsKey(v));
            Check(() => !_adjacenylist.Keys.Any(x => x.Id == v.Id));

            _adjacenylist.Add(v, new List<Edge>());
        }

        /// <summary>
        /// Removes the specified vertex from the graph
        /// </summary>
        /// <param name="v">The vertex to remove</param>
        public void RemoveVertex(Vertex v)
        {
            _adjacenylist.Remove(v);

            foreach (var adjacent in _adjacenylist)
            {
                Edge? e = GetEdge(adjacent.Key, v);

                if (e != null)
                {
                    adjacent.Value.Remove(e);
                }
            }
        }

        /// <param name="from">The start vertex of the edge</param>
        /// <param name="to">The end vertex of the edge</param>
        /// <returns>The specified edge</returns>
        public Edge? GetEdge(Vertex from, Vertex to)
        {
            Check(() => _adjacenylist.ContainsKey(from));

            return _adjacenylist[from].FirstOrDefault(e => e.To == to);
        }

        /// <summary>
        /// Returns the specified vertex of the graph if exists
        /// </summary>
        /// <param name="id">The vertex identifier</param>
        /// <returns>The specified vertex</returns>
        public Vertex? GetVertexById(int id) 
        { 
            return _adjacenylist.Keys.FirstOrDefault(v => v.Id == id);
        }

        /// <summary>
        /// Returns the weight of the specified edge if exists
        /// </summary>
        /// <param name="from">The start vertex of the edge</param>
        /// <param name="to">The end vertex of the edge</param>
        /// <returns>The weight of the specified edge</returns>
        public short? GetWeight(Vertex from, Vertex to)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));

            Edge? e = GetEdge(from, to);

            return e?.Weight;
        }

        /// <summary>
        /// Creates the adjacency matrix representation of the graph
        /// </summary>
        /// <returns>The adjacency matrix representation</returns>
        public int[,] ToAdjacencyMatrix()
        {
            int[,] adjacencyMatrix = new int[VertexCount, VertexCount];

            for (int i = 0; i < VertexCount; ++i)
            {
                for (int j = 0; j < VertexCount; ++j)
                {
                    Edge? e = GetEdge(Vertices[i], Vertices[j]);

                    adjacencyMatrix[i, j] = e != null ? e.Weight : int.MaxValue;
                }

                adjacencyMatrix[i, i] = 0;
            }

            return adjacencyMatrix;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Throws an exception if the condition is false
        /// </summary>
        /// <param name="cond">The condition</param>
        /// <exception cref="InvalidOperationException"></exception>
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
