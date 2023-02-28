namespace Floyd_Warshall_Model.Graph
{
    public class DirectedGraph : GraphBase
    {
        public DirectedGraph(): base() { }

        public override void AddEdge(Vertex from, Vertex to, int weight)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));
            Check(() => to != from);
            Check(() => GetEdge(from, to) == null);

            _adjacenylist[from].Add(new Edge(from, to, weight));
        }

        public override void RemoveEdge(Vertex from, Vertex to)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));

            Edge e = GetEdge(from, to);

            if (e != null)
            {
                _adjacenylist[from].Remove(e);
            }
        }

        public override void UpdateWeight(Vertex from, Vertex to, int weight)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));

            Edge e = GetEdge(from, to);

            if (e != null)
            {
                e.Weight = weight;
            }
        }

        public override void IncrementWeight(Vertex from, Vertex to)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));

            Edge e = GetEdge(from, to);

            if (e != null)
            {
                ++e.Weight;
            }
        }
    }
}
