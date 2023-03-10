using Floyd_Warshall_Model.Persistence;

namespace Floyd_Warshall_Model.Events
{
    public class GraphLoadedEventArgs : EventArgs
    {
        public GraphLoadedEventArgs(IEnumerable<VertexLocation> locations)
        {
            VertexLocations = locations;
        }

        public IEnumerable<VertexLocation> VertexLocations { get; private set; }
    }
}
