namespace Floyd_Warshall_Model.Model.Graph
{
    public class UndirectedGraph : GraphBase
    {
        public UndirectedGraph() : base() { }

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

            if (weight < minValue || weight > maxValue)
            {
                throw new OverflowException();
            }

            Edge? e1 = GetEdge(from, to);
            Edge? e2 = GetEdge(to, from);

            if (e1 != null && e2 != null)
            {
                e1.Weight = weight;
                e2.Weight = weight;
            }
        }

        public override void IncrementWeight(Vertex from, Vertex to)
        {
            Check(() => _adjacenylist.ContainsKey(from));
            Check(() => _adjacenylist.ContainsKey(to));

            Edge? e1 = GetEdge(from, to);
            Edge? e2 = GetEdge(to, from);

            if (e1 != null && e2 != null)
            {
                if (e1.Weight >= maxValue || e2.Weight >= maxValue)
                {
                    throw new OverflowException();
                }
                ++e1.Weight;
                ++e2.Weight;
            }
        }

    }
}
