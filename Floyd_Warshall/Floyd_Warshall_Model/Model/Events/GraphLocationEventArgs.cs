using Floyd_Warshall_Model.Persistence;

namespace Floyd_Warshall_Model.Model.Events
{
    /// <summary>
    /// Type of the graph location event argument
    /// </summary>
    public class GraphLocationEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor of the graph location event argument
        /// </summary>
        /// <param name="locations">The vertex locations</param>
        public GraphLocationEventArgs(IEnumerable<VertexLocation> locations)
        {
            VertexLocations = locations;
        }

        /// <summary>
        /// Gets the vertex locations
        /// </summary>
        public IEnumerable<VertexLocation> VertexLocations { get; }
    }
}
