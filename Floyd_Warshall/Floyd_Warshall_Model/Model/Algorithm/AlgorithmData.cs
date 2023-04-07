namespace Floyd_Warshall_Model.Model.Algorithm
{
    public class AlgorithmData
    {
        public AlgorithmData(int[,] d, int[,] pi, ICollection<Tuple<int, int>>? changesD = null, ICollection<Tuple<int, int>>? changesPi = null)
        {
            D = d;
            Pi = pi;
            ChangesD = changesD;
            ChangesPi = changesPi;
        }

        public int[,] D { get; }
        public int[,] Pi { get; }
        public ICollection<Tuple<int, int>>? ChangesD { get; }
        public ICollection<Tuple<int, int>>? ChangesPi { get; }
    }
}
