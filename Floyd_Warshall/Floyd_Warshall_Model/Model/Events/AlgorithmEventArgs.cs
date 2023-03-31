using Floyd_Warshall_Model.Model.Algorithm;

namespace Floyd_Warshall_Model.Model.Events
{
    public class AlgorithmEventArgs : EventArgs
    {
        public AlgorithmEventArgs(int[,] d, int[,] pi)
        {
            Data = new AlgorithmData(d, pi);
        }

        public AlgorithmData Data { get; }
    }
}
