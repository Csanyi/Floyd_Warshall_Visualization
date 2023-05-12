namespace Floyd_Warshall_Model.Model.Events
{
	/// <summary>
	/// Type of the algorithm initialize event argument
	/// </summary>
	public class AlgorithmInitEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor of the algorithm initialize event argument
        /// </summary>
        /// <param name="d">The D matrix</param>
        /// <param name="pi">The Pi matrix</param>
        public AlgorithmInitEventArgs(int[,] d, int[,] pi)
        {
            D = d;
            Pi = pi;
        }

        /// <summary>
        /// Gets the D martrix
        /// </summary>
        public int[,] D { get; }

        /// <summary>
        /// Gets the Pi matrix
        /// </summary>
        public int[,] Pi { get; }
    }
}
