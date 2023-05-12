namespace Floyd_Warshall_Model.Persistence
{
	/// <summary>
	/// Type to store vertex data
	/// </summary>
	public class VertexData
    {
        /// <summary>
        /// Constructor of the vertex data
        /// </summary>
        /// <param name="id">Vertex id</param>
        /// <param name="x">Vertex x coord</param>
        /// <param name="y">Vertex y coord</param>
        public VertexData(int id, double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets the vertex id
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the x coord of the vertex
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the y coord of the vertex
        /// </summary>
        public double Y { get; }
    }
}
