namespace Floyd_Warshall_Model.Model.Events
{
	/// <summary>
	/// Type of the edge added event argument
	/// </summary>
	public class EdgeAddedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor of the edge added event argument
        /// </summary>
        /// <param name="id">Id of the added edge</param>
        /// <param name="from">Id of the start vertex</param>
        /// <param name="to">Id of the end vertex</param>
        /// <param name="weight">Weight of the edge</param>
        public EdgeAddedEventArgs(int id, int from, int to, short weight)
        {
            Id = id;
            From = from;
            To = to;
            Weight = weight;
        }

        /// <summary>
        /// Gets the edge id
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the start vertex id
        /// </summary>
        public int From { get; }

        /// <summary>
        /// Gets the end vertex id
        /// </summary>
        public int To { get; }

        /// <summary>
        /// Gets the edge weight
        /// </summary>
        public short Weight { get; }
    }
}
