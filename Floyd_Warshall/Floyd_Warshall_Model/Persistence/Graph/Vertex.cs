namespace Floyd_Warshall_Model.Persistence.Graph
{
    /// <summary>
    /// Type of the graph vertex
    /// </summary>
    public class Vertex
    {
        /// <summary>
        /// Constuctor of the vertex
        /// </summary>
        /// <param name="id">Vertex identifier</param>
        public Vertex(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Gets the vertex id
        /// </summary>
        public int Id { get; }
    }
}
