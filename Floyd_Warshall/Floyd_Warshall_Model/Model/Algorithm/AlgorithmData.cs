namespace Floyd_Warshall_Model.Model.Algorithm
{
    /// <summary>
    /// Type to store the algorithm data
    /// </summary>
    public class AlgorithmData
    {
        /// <summary>
        /// Constructor of the AlgorithmData
        /// </summary>
        /// <param name="d">The D matrix</param>
        /// <param name="pi">The Pi matrix</param>
        /// <param name="prevD">The previous D matrix</param>
        /// <param name="prevPi">The previous Pi matrix</param>
        /// <param name="changesD">The changes of the D matrix</param>
        /// <param name="changesPi">The changes of the Pi matrix</param>
        public AlgorithmData(int[,] d, int[,] pi, int[,] prevD, int[,] prevPi,
            ICollection<Change> changesD, ICollection<Change> changesPi)
        {
            D = d;
            Pi = pi;
            PrevD = prevD;
            PrevPi = prevPi;
            ChangesD = changesD;
            ChangesPi = changesPi;
        }

        /// <summary>
        /// Gets the D matrix
        /// </summary>
        public int[,] D { get; }

        /// <summary>
        /// Gets the Pi matrix
        /// </summary>
        public int[,] Pi { get; }

        /// <summary>
        /// Gets the previous D matrix
        /// </summary>
        public int[,] PrevD { get; }

        /// <summary>
        /// Gets the previous Pi matrix
        /// </summary>
        public int[,] PrevPi { get; }

        /// <summary>
        /// Gets the changes of the D matrix
        /// </summary>
        public ICollection<Change> ChangesD { get; }

        /// <summary>
        /// Gets the changes of the Pi matrix
        /// </summary>
        public ICollection<Change> ChangesPi { get; }
    }
}
