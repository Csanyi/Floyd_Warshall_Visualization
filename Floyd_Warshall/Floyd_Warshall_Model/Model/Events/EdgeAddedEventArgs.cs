namespace Floyd_Warshall_Model.Model.Events
{
    public class EdgeAddedEventArgs : EventArgs
    {
        public EdgeAddedEventArgs(int id, int from, int to, short weight)
        {
            Id = id;
            From = from;
            To = to;
            Weight = weight;
        }

        public int Id { get; }
        public int From { get; }
        public int To { get; }
        public short Weight { get; }
    }
}
