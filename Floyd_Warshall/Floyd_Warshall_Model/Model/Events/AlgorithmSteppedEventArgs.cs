using Floyd_Warshall_Model.Model.Algorithm;

namespace Floyd_Warshall_Model.Model.Events
{
    public class AlgorithmSteppedEventArgs : EventArgs
    {
        public AlgorithmSteppedEventArgs(ICollection<ChangeValue> d, ICollection<ChangeValue> pi, 
            ICollection<ChangeValue> prevD, ICollection<ChangeValue> prevPi)
        {
            ChangeD = d;
            ChangePi = pi;
            ChangePrevD = prevD;
            ChangePrevPi = prevPi;
        }

        public ICollection<ChangeValue> ChangeD { get; }
        public ICollection<ChangeValue> ChangePi { get; }
        public ICollection<ChangeValue> ChangePrevD { get; }
        public ICollection<ChangeValue> ChangePrevPi { get; }
    }
}
