namespace Floyd_Warshall_Model.Persistence
{
    public class VertexData
    {
        public VertexData(int id, double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public int Id { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
    }
}
