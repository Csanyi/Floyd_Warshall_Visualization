namespace Floyd_Warshall_Model.Graph
{
    public class Edge
    {
        public Edge(Vertex from, Vertex to, int weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }

        public Vertex From { get; }

        public Vertex To { get; }

        public int Weight { get; set; }
    }
}
