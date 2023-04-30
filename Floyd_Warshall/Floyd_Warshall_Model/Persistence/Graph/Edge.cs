namespace Floyd_Warshall_Model.Persistence.Graph
{
    /// <summary>
    /// Type of the graph edge
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// Constructor of the edge
        /// </summary>
        /// <param name="from">Start vertex</param>
        /// <param name="to">End vertex</param>
        /// <param name="weight">The weight of the edge</param>
        public Edge(Vertex from, Vertex to, short weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }

        /// <summary>
        /// Gets the edge start vertex
        /// </summary>
        public Vertex From { get; }

        /// <summary>
        /// Gets the edge end vertex
        /// </summary>
        public Vertex To { get; }

        /// <summary>
        /// Gets the edge weight
        /// </summary>
        public short Weight { get; set; }
    }
}
