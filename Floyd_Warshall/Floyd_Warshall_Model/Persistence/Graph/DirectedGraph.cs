namespace Floyd_Warshall_Model.Persistence.Graph
{
	/// <summary>
	/// Type of the directed graph
	/// </summary>
	public class DirectedGraph : GraphBase
    {
        /// <summary>
        /// Constructor of the undirected graph
        /// </summary>
        public DirectedGraph() : base() { }

        /// <summary>
        /// Gets the direction of the graph
        /// </summary>
        public override bool IsDirected => true;

        /// <summary>
        /// Adds an an edge to the graph
        /// </summary>
        /// <param name="from">The start vertex of the edge</param>
        /// <param name="to">The end vertex of the edge</param>
        /// <param name="weight">The weight of the edge</param>
        /// <exception cref="OverflowException"></exception>
        public override void AddEdge(Vertex from, Vertex to, short weight)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));
            Check(() => to != from);
            Check(() => GetEdge(from, to) == null);

            if (weight < MinValue || weight > MaxValue)
            {
                throw new OverflowException();
            }

            _adjacenylist[from].Add(new Edge(from, to, weight));
        }

        /// <summary>
        /// Removes the specified edge from the graph
        /// </summary>
        /// <param name="from">The start vertex of the edge</param>
        /// <param name="to">The end vertex of the edge</param>
        public override void RemoveEdge(Vertex from, Vertex to)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));

            Edge? e = GetEdge(from, to);

            if (e != null)
            {
                _adjacenylist[from].Remove(e);
            }
        }

        /// <summary>
        /// Updates the weight of the specified edge to the specified  value
        /// </summary>
        /// <param name="from">The start vertex of the edge</param>
        /// <param name="to">The end vertex of the edge</param>
        /// <param name="weight">The new weight</param>
        /// <exception cref="OverflowException"></exception>
        public override void UpdateWeight(Vertex from, Vertex to, short weight)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));

            if (weight < MinValue || weight > MaxValue)
            {
                throw new OverflowException();
            }

            Edge? e = GetEdge(from, to);

            if (e != null)
            {
                e.Weight = weight;
            }
        }

        /// <summary>
        /// Increases by one the weight of the specified edge
        /// </summary>
        /// <param name="from">The start vertex of the edge</param>
        /// <param name="to">The end vertex of the edge</param>
        /// <exception cref="OverflowException"></exception>
        public override void IncrementWeight(Vertex from, Vertex to)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));

            Edge? e = GetEdge(from, to);

            if (e != null)
            {
                if (e.Weight >= MaxValue)
                {
                    throw new OverflowException();
                }
                ++e.Weight;
            }
        }
    }
}
