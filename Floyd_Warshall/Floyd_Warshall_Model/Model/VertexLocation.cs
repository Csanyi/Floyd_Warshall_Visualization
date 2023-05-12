namespace Floyd_Warshall_Model.Model
{
	/// <summary>
	/// Type to store the vertex location
	/// </summary>
	public class VertexLocation
    {
        /// <summary>
        /// Constructor of the VertexLocation
        /// </summary>
        /// <param name="id">Vertex id</param>
        /// <param name="x">Vertex x coord</param>
        /// <param name="y">Vertex y coord</param>
        public VertexLocation(int id, double x, double y)
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
        /// Gets the vertex x coord
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the vertex y coord
        /// </summary>
        public double Y { get; }
    }
}
