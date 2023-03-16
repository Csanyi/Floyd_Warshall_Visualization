namespace Floyd_Warshall_Model
{
    public class AlgorithmData
    {
        public AlgorithmData(int[,] d, int[,] pi)
        {
            D = d;
            Pi = pi;
        }

        public int[,] D { get; private set; }
        public int[,] Pi { get; private set; }
    }
}
