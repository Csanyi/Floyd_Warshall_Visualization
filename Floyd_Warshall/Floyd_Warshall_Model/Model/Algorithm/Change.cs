namespace Floyd_Warshall_Model.Model.Algorithm
{
    public class Change
    {
        public Change(int i, int j, int oldValue, int newValue)
        {
            Pos = new ChangePos(i, j);
            OldValue = oldValue;
            NewValue = newValue;
        }

        public ChangePos Pos { get; }
        public int OldValue { get; }
        public int NewValue { get; }
    }

    public class ChangePos: IEquatable<ChangePos>
    {
        public ChangePos(int i, int j)
        {
            I = i;
            J = j;
        }

        public int I { get; }
        public int J { get; }

        public override int GetHashCode()
        {
            return HashCode.Combine(I, J); ;
        }

        public bool Equals(ChangePos? other)
        {
            return other != null && other.I == I && other.J == J;
        }
    }
}
