namespace Floyd_Warshall_Model.Model.Algorithm
{
    public class Change : IEquatable<Change>
    {
        public Change(int i, int j)
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

        public bool Equals(Change? other)
        {
            return other != null && other.I == I && other.J == J;
        }
    }


    public class ChangeValue : Change
    {
        public ChangeValue(int i, int j, int value): base(i, j)
        {
            Value = value;
        }

        public int Value { get; }
    }

    public class ChangeOldNew : Change
    {
        public ChangeOldNew(int i, int j, int oldValue, int newValue): base(i, j)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public int OldValue { get; }
        public int NewValue { get; }
    }
}
