namespace Floyd_Warshall_Model.Model.Events
{
    public class NewGraphEventArgs : EventArgs
    {
        public NewGraphEventArgs(bool isDirected)
        {
            IsDirected = isDirected;
        }

        public bool IsDirected { get; private set; }
    }
}
