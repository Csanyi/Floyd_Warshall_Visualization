using Floyd_Warshall_Model.Graph;

namespace Floyd_Warshall_Model.Persistence
{
    public class VertexLocation
    {
        public VertexLocation(Vertex v, double x, double y)
        {
            Vertex = v;
            X = x;
            Y = y;
        }

        public Vertex Vertex { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
    }
}
