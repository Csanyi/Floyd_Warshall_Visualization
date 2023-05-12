namespace Floyd_Warshall_Model.Model.Events
{
	/// <summary>
	/// Type of the new grah event argument
	/// </summary>
	public class NewGraphEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor of the new graph event argument
        /// </summary>
        /// <param name="isDirected">The direction of the new graph</param>
        public NewGraphEventArgs(bool isDirected)
        {
            IsDirected = isDirected;
        }

        /// <summary>
        /// Gets the graph direction
        /// </summary>
        public bool IsDirected { get; }
    }
}
