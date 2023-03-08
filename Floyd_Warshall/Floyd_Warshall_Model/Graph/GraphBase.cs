namespace Floyd_Warshall_Model.Graph
{
    public abstract class GraphBase
    {
        protected IDictionary<Vertex, ICollection<Edge>> _adjacenylist;

        protected GraphBase()
        { 
            _adjacenylist = new Dictionary<Vertex, ICollection<Edge>>();
        }

        public abstract bool IsDirected { get; }

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

            foreach(var adjacent in _adjacenylist)
            {
                Edge? e = GetEdge(adjacent.Key, v);

                if(e != null)
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

        public List<Vertex> GetNeighbours(Vertex from) => _adjacenylist[from].Select(e => e.To).ToList();

        public List<Vertex> GetVertices() => _adjacenylist.Keys.ToList();

        public List<Edge> GetEdges()
        {
            List<Edge> edges = new List<Edge>();

            foreach(var v in _adjacenylist)
            {
                edges.AddRange(v.Value);
            }

            return edges;
        }

        public int GetEdgeCount() => _adjacenylist.Sum(x => x.Value.Count);

        public int GetVertexCount() => _adjacenylist.Count;

        public Vertex? GetVertexById(int id) => _adjacenylist.Keys.FirstOrDefault(v => v.Id == id);

        public short GetWeight(Vertex from, Vertex to)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));

            return _adjacenylist[from].First(e => e.To == to).Weight;
        }

        public int[,] ToAdjacencyMatrix()
        {
            int size = GetVertexCount();
            IList<Vertex> vertices = GetVertices();

            int[,] adjacencyMatrix = new int[size, size];

            for(int i = 0;  i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    Edge? e = GetEdge(vertices[i], vertices[j]);

                    adjacencyMatrix[i, j] = (e != null) ? e.Weight : int.MaxValue; 
                }

                adjacencyMatrix[i, i] = 0;
            }

            return adjacencyMatrix;
        }

        protected static void Check(Func<bool> cond)
        {
            if(!cond())
            {
                throw new InvalidOperationException();
            }
        }
    }
}
