namespace Floyd_Warshall_Model.Model.Events
{
    /// <summary>
    /// Type of the edge updated event argument
    /// </summary>
    public class EdgeUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor of the edge updated event argument
        /// </summary>
        /// <param name="from">Start vertex id of the updated edge</param>
        /// <param name="to">End vertex id of the updated edge</param>
        public EdgeUpdatedEventArgs(int from, int to)
        {
            From = from;
            To = to;
        }

        /// <summary>
        /// Gets the start vertex id
        /// </summary>
        public int From { get; }

        /// <summary>
        /// Gets the end vertex id
        /// </summary>
        public int To { get; }
    }
}
