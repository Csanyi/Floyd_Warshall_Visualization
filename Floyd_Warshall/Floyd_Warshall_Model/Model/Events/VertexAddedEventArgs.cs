namespace Floyd_Warshall_Model.Model.Events
{
    public class VertexAddedEventArgs : EventArgs
    {
        public VertexAddedEventArgs(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
    }
}
