using Floyd_Warshall_Model.Model.Algorithm;

namespace Floyd_Warshall_Model.Model.Events
{
    public class AlgorithmInitEventArgs : EventArgs
    {
        public AlgorithmInitEventArgs(int[,] d, int[,] pi)
        {
            D = d;
            Pi = pi;
        }

        public int[,] D { get; }
        public int[,] Pi { get; }
    }
}
