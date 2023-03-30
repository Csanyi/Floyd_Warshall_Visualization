using Floyd_Warshall_Model.Model.Graph;

namespace Floyd_Warshall_Test
{
    [TestClass]
    public class GraphTest
    {
        private GraphBase _graph = null!;

        [TestInitialize]
        public void Initialize()
        {
            _graph = new DirectedGraph();
        }


        [TestMethod]
        public void EmptyGraph()
        {
            Assert.AreEqual(_graph.GetEdgeCount(), 0);
            Assert.AreEqual(_graph.GetVertexCount(), 0);
        }

        [TestMethod]
        public void AddVertex()
        {
            _graph.AddVertex(new Vertex(1));

            Assert.AreEqual(_graph.GetEdgeCount(), 0);
            Assert.AreEqual(_graph.GetVertexCount(), 1);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void AddVertexException()
        {
            Vertex v = new Vertex(1);

            _graph.AddVertex(v);
            _graph.AddVertex(v);
        }

        [TestMethod]
        public void AddEdge()
        {
            Vertex v1 = new Vertex(1);
            Vertex v2 = new Vertex(2);

            _graph.AddVertex(v1);
            _graph.AddVertex(v2);
            _graph.AddEdge(v1, v2, 10);

            Assert.AreEqual(_graph.GetEdgeCount(), 1);
            Assert.AreEqual(_graph.GetVertexCount(), 2);
            Assert.AreEqual(_graph.GetWeight(v1,v2), 10);

            _graph.AddEdge(v2, v1, 11);

            Assert.AreEqual(_graph.GetEdgeCount(), 2);
            Assert.AreEqual(_graph.GetVertexCount(), 2);
            Assert.AreEqual(_graph.GetWeight(v2, v1), 11);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void AddEdgeException()
        {
            Vertex v1 = new Vertex(1);
            Vertex v2 = new Vertex(2);

            _graph.AddVertex(v1);
            _graph.AddEdge(v1, v2, 10);
        }


        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void AddLoopEdgeException()
        {
            Vertex v1 = new Vertex(1);

            _graph.AddVertex(v1);
            _graph.AddEdge(v1, v1, 10);
        }

        [TestMethod]
        public void RemoveEdge()
        {
            Vertex v1 = new Vertex(1);
            Vertex v2 = new Vertex(2);

            _graph.AddVertex(v1);
            _graph.AddVertex(v2);
            _graph.AddEdge(v1, v2, 10);

            Assert.AreEqual(_graph.GetVertexCount(), 2);
            Assert.AreEqual(_graph.GetEdgeCount(), 1);

            _graph.RemoveEdge(v1, v2);

            Assert.AreEqual(_graph.GetVertexCount(), 2);
            Assert.AreEqual(_graph.GetEdgeCount(), 0);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void RemoveEdgeException()
        {
            Vertex v1 = new Vertex(1);
            Vertex v2 = new Vertex(2);

            _graph.AddVertex(v1);
            _graph.RemoveEdge(v1, v2);
        }

        [TestMethod]
        public void RemoveVertex()
        {
            Vertex v1 = new Vertex(1);
            Vertex v2 = new Vertex(2);

            _graph.AddVertex(v1);
            _graph.AddVertex(v2);
            _graph.AddEdge(v1, v2, 10);
            _graph.AddEdge(v2, v1, 10);
            _graph.RemoveVertex(v1);

            Assert.AreEqual(_graph.GetVertexCount(), 1);
            Assert.AreEqual(_graph.GetEdgeCount(), 0);
        }
    }
}