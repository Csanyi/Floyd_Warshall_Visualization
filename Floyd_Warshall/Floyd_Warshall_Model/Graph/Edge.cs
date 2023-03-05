namespace Floyd_Warshall_Model.Graph
{
    public class Edge
    {
        public Edge(Vertex from, Vertex to, short weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }

        public Vertex From { get; }

        public Vertex To { get; }

        public short Weight { get; set; }
    }
}
