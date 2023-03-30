namespace Floyd_Warshall_Model.Model.Algorithm
{
    public class AlgorithmData
    {
        public AlgorithmData(int[,] d, int[,] pi, ICollection<Tuple<int, int>> changes = null!)
        {
            D = d;
            Pi = pi;
            Changes = changes;
        }

        public int[,] D { get; private set; }
        public int[,] Pi { get; private set; }
        public ICollection<Tuple<int, int>> Changes { get; private set; }
    }
}
