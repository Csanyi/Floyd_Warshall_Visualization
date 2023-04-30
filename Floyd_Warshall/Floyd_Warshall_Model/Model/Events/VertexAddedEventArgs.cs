namespace Floyd_Warshall_Model.Model.Events
{
    /// <summary>
    /// Type of the vertex added event argument
    /// </summary>
    public class VertexAddedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor of the vertex added event argument
        /// </summary>
        /// <param name="id">The id of the added vertex</param>
        public VertexAddedEventArgs(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Gets the vertex id
        /// </summary>
        public int Id { get; }
    }
}
