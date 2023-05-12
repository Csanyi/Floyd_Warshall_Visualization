namespace Floyd_Warshall_Model.Model.Algorithm
{
	/// <summary>
	/// Type to store a change
	/// </summary>
	public class Change : IEquatable<Change>
    {
        /// <summary>
        /// Constructor of the Change
        /// </summary>
        /// <param name="i">X coord of the change</param>
        /// <param name="j">Y coord of the change</param>
        public Change(int i, int j)
        {
            I = i;
            J = j;
        }

        /// <summary>
        /// Gets the x coord
        /// </summary>
        public int I { get; }

        /// <summary>
        /// Gets the y coord
        /// </summary>
        public int J { get; }

        public override int GetHashCode()
        {
            return HashCode.Combine(I, J); ;
        }

        public override bool Equals(object? obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Change c = (Change)obj;
                return (I == c.I) && (J == c.J);
            }
        }

        public bool Equals(Change? other)
        {
            return other != null && other.I == I && other.J == J;
        }
    }

    /// <summary>
    /// Type to store a change with value
    /// </summary>
    public class ChangeValue : Change
    {
        /// <summary>
        /// Constructor of the ChangeValue
        /// </summary>
        /// <param name="i">X coord of the change</param>
        /// <param name="j">Y coord of the change</param>
        /// <param name="value">The value</param>
        public ChangeValue(int i, int j, int value): base(i, j)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value
        /// </summary>
        public int Value { get; }
    }

    /// <summary>
    /// Type to stroe a change with old and new value
    /// </summary>
    public class ChangeOldNew : Change
    {
        /// <summary>
        /// Consturctor of the ChangeOldNew
        /// </summary>
        /// <param name="i">X coord of the change</param>
        /// <param name="j">Y coord of the change</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value/param>
        public ChangeOldNew(int i, int j, int oldValue, int newValue): base(i, j)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Gets the old value
        /// </summary>
        public int OldValue { get; }

        /// <summary>
        /// Gets the new value
        /// </summary>
        public int NewValue { get; }
    }
}
