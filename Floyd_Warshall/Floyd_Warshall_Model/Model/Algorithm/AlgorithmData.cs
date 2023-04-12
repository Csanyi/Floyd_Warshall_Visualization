namespace Floyd_Warshall_Model.Model.Algorithm
{
    public class AlgorithmData
    {
        public AlgorithmData(int[,] d, int[,] pi, int[,] prevD, int[,] prevPi,
            ICollection<ChangePos> changesD, ICollection<ChangePos> changesPi)
        {
            D = d;
            Pi = pi;
            PrevD = prevD;
            PrevPi = prevPi;
            ChangesD = changesD;
            ChangesPi = changesPi;
        }

        public int[,] D { get; }
        public int[,] Pi { get; }
        public int[,] PrevD { get; }
        public int[,] PrevPi { get; }
        public ICollection<ChangePos> ChangesD { get; }
        public ICollection<ChangePos> ChangesPi { get; }
    }
}
