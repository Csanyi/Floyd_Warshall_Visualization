namespace Floyd_Warshall_Model.Persistence.Graph
{
    public class DirectedGraph : GraphBase
    {
        public DirectedGraph() : base() { }

        public override bool IsDirected => true;

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
