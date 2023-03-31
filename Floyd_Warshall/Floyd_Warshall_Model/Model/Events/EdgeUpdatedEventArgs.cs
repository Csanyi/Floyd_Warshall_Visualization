namespace Floyd_Warshall_Model.Model.Events
{
    public class EdgeUpdatedEventArgs : EventArgs
    {
        public EdgeUpdatedEventArgs(int from, int to)
        {
            From = from;
            To = to;
        }

        public int From { get; }
        public int To { get; }
    }
}
