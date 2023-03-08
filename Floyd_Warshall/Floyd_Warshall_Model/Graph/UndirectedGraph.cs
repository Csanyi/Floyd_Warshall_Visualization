namespace Floyd_Warshall_Model.Graph
{
    public class UndirectedGraph : GraphBase
    {
        public UndirectedGraph(): base() { }

        public override bool IsDirected => false;

        public override void AddEdge(Vertex from, Vertex to, short weight)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));
            Check(() => to != from);
            Check(() => GetEdge(from, to) == null);

            _adjacenylist[from].Add(new Edge(from, to, weight));
            _adjacenylist[to].Add(new Edge(to, from, weight));
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

            e = GetEdge(to, from);
            if (e != null)
            {
                _adjacenylist[to].Remove(e);
            }
        }

        public override void UpdateWeight(Vertex from, Vertex to, short weight)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));

            Edge? e = GetEdge(from, to);
            if (e != null && e.Weight < short.MaxValue)
            {
                e.Weight = weight;
            }
            
            e = GetEdge(to, from);
            if (e != null && e.Weight < short.MaxValue)
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
                ++e.Weight;
            }

            e = GetEdge(to, from);
            if (e != null)
            {
                ++e.Weight;
            }
        }

    }
}
