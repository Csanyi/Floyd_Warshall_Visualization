using Floyd_Warshall_Model.Model.Algorithm;

namespace Floyd_Warshall_Model.Model.Events
{
	/// <summary>
	/// Type of the algorithm step event argument
	/// </summary>
	public class AlgorithmSteppedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor of the algorithm step event argument
        /// </summary>
        /// <param name="d">Changes of the D matrix</param>
        /// <param name="pi">>Changes of the Pi matrix</param>
        /// <param name="prevD">>Changes of the previous D matrix</param>
        /// <param name="prevPi">>Changes of the previous Pi matrix</param>
        public AlgorithmSteppedEventArgs(ICollection<ChangeValue> d, ICollection<ChangeValue> pi, 
            ICollection<ChangeValue> prevD, ICollection<ChangeValue> prevPi)
        {
            ChangeD = d;
            ChangePi = pi;
            ChangePrevD = prevD;
            ChangePrevPi = prevPi;
        }

        /// <summary>
        /// Gets the changes of the D matrix
        /// </summary>
        public ICollection<ChangeValue> ChangeD { get; }

        /// <summary>
        /// Gets the changes of the Pi matrix
        /// </summary>
        public ICollection<ChangeValue> ChangePi { get; }

        /// <summary>
        /// Gets the changes of the previous D matrix
        /// </summary>
        public ICollection<ChangeValue> ChangePrevD { get; }

        /// <summary>
        /// Gets the changes of the previous Pi matrix
        /// </summary>
        public ICollection<ChangeValue> ChangePrevPi { get; }
    }
}
