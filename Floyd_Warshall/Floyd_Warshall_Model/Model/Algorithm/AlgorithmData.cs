namespace Floyd_Warshall_Model.Model.Algorithm
{
    public class AlgorithmData
    {
        public AlgorithmData(int[,] d, int[,] pi, ICollection<Tuple<int, int>>? changes = null)
        {
            D = d;
            Pi = pi;
            Changes = changes;
        }

        public int[,] D { get; }
        public int[,] Pi { get; }
        public ICollection<Tuple<int, int>>? Changes { get; }
    }
}
