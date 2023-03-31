using Floyd_Warshall_Model.Persistence;

namespace Floyd_Warshall_Model.Model.Events
{
    public class GraphLocationEventArgs : EventArgs
    {
        public GraphLocationEventArgs(IEnumerable<VertexLocation> locations)
        {
            VertexLocations = locations;
        }

        public IEnumerable<VertexLocation> VertexLocations { get; }
    }
}
