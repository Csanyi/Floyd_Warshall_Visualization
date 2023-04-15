using Floyd_Warshall_Model.Model;
using Floyd_Warshall_Model.Model.Algorithm;
using Floyd_Warshall_Model.Model.Events;
using Floyd_Warshall_Model.Persistence;
using Floyd_Warshall_Model.Persistence.Graph;
using Moq;
using System.Data;

namespace Floyd_Warshall_Test
{
    [TestClass]
    public class GraphModelTest
    {
        #region Fields

        private GraphModel _model = null!;
        private Mock<IGraphDataAccess> _mock = null!;
        private GraphBase _mockedGraph = null!;

        #endregion

        #region Initialize

        [TestInitialize]
        public void Initialize()
        {
            _mockedGraph = new UndirectedGraph();
            List<Vertex> vertices = new List<Vertex>();

            for (int i = 0; i < 6; ++i)
            {
                Vertex v = new Vertex(i);
                _mockedGraph.AddVertex(v);
                vertices.Add(v);
            }

            _mockedGraph.AddEdge(vertices[0], vertices[2], 10);
            _mockedGraph.AddEdge(vertices[0], vertices[4], -5);
            _mockedGraph.AddEdge(vertices[1], vertices[2], 0);
            _mockedGraph.AddEdge(vertices[1], vertices[3], 342);
            _mockedGraph.AddEdge(vertices[2], vertices[3], 9999);
            _mockedGraph.AddEdge(vertices[2], vertices[5], -4242);
            _mockedGraph.AddEdge(vertices[3], vertices[4], -9999);
            _mockedGraph.AddEdge(vertices[4], vertices[5], 24);

            GraphData data = new GraphData(_mockedGraph, new List<VertexData>());

            _mock = new Mock<IGraphDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<String>())).Returns(() => Task.FromResult(data));

            _model = new GraphModel(_mock.Object);

            _model.AlgorithmInitialized += Model_AlgorithmInitialized;
            _model.AlgorithmStepped += Model_AlgorithmStepped;
            _model.AlgorithmSteppedBack += Model_AlogorithmSteppedBack;
            _model.RouteCreated += Model_RouteCreated;
            _model.AlgorithmEnded += Model_AlgorithmEnded;
            _model.NegativeCycleFound += Model_NegativeCycleFound;
        }

        #endregion

        #region Graph tests

        [TestMethod]
        public void NewGraph()
        {
            _model.NewGraph(false);
            Assert.IsFalse(_model.IsDirected);
            Assert.AreEqual(0, _model.GetVertexCount());
            Assert.AreEqual(0, _model.GetEdgeCount());

            _model.NewGraph(true);
            Assert.IsTrue(_model.IsDirected);
            Assert.AreEqual(0, _model.GetVertexCount());
            Assert.AreEqual(0, _model.GetEdgeCount());
        }

        [TestMethod]
        public void AddVertex()
        {
            _model.AddVertex();

            Assert.AreEqual(1, _model.GetVertexCount());
        }

        [TestMethod]
        [DataRow((short)10)]
        [DataRow((short)9999)]
        [DataRow((short)-9999)]
        public void AddUndirectedEdge(short weight)
        {
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], weight);

            Assert.AreEqual(2, _model.GetEdgeCount());
            Assert.AreEqual(weight, _model.GetWeight(ids[0], ids[1]));
            Assert.AreEqual(weight, _model.GetWeight(ids[1], ids[0]));
        }

        [TestMethod]
        [DataRow((short)10)]
        [DataRow((short)9999)]
        [DataRow((short)-9999)]
        public void AddDirectedEdge(short weight)
        {
            _model.NewGraph(true);
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], weight);

            Assert.AreEqual(1, _model.GetEdgeCount());
            Assert.AreEqual(weight, _model.GetWeight(ids[0], ids[1]));
            Assert.IsNull(_model.GetWeight(ids[1], ids[0]));
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        [DataRow((short)10000, false)]
        [DataRow((short)-10000, false)]
        [DataRow((short)10000, true)]
        [DataRow((short)-10000, true)]
        public void AddEdgeOverFlow(short weight, bool isDirected)
        {
            _model.NewGraph(isDirected);
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();
            try
            {
                _model.AddEdge(ids[0], ids[1], weight);
            } catch (OverflowException) 
            {
                Assert.AreEqual(0, _model.GetEdgeCount());
                throw;
            }
        }

        [TestMethod]
        public void AddUndirectedEdgeTwice()
        {
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 10);
            _model.AddEdge(ids[0], ids[1], 10);

            Assert.AreEqual(2, _model.GetEdgeCount());
        }

        [TestMethod]
        public void AddDirectedEdgeTwice()
        {
            _model.NewGraph(true);
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 10);
            _model.AddEdge(ids[0], ids[1], 10);

            Assert.AreEqual(1, _model.GetEdgeCount());
        }

        [TestMethod]
        public void AddUndirectedEdgeTwice2()
        {
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 10);
            _model.AddEdge(ids[1], ids[0], 10);

            Assert.AreEqual(2, _model.GetEdgeCount());
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void AddEdgeToNonExistVertex(bool isDirected)
        {
            _model.NewGraph(isDirected);
            _model.AddEdge(1, 2, 10);

            Assert.AreEqual(0, _model.GetEdgeCount());
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void AddLoopEdge(bool isDirected)
        {
            _model.NewGraph(isDirected);
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[0], 10);

            Assert.AreEqual(0, _model.GetEdgeCount());
        }

        [TestMethod]
        public void RemoveVertex()
        {
            _model.AddVertex();

            Assert.AreEqual(1, _model.GetVertexCount());

            List<int> ids = _model.GetVertexIds();

            int deleteId = ids[0];

            _model.RemoveVertex(deleteId);

            Assert.AreEqual(0, _model.GetVertexCount());
            Assert.IsFalse(_model.GetVertexIds().Contains(deleteId));
        }

        [TestMethod]
        public void RemoveNonExistVertex()
        {
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.RemoveVertex(ids.Last() + 1);

            Assert.AreEqual(1, _model.GetVertexCount());
        }

        [TestMethod]
        public void RemoveVertexWithUndirectedEdge()
        {
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 10);

            Assert.AreEqual(2, _model.GetVertexCount());
            Assert.AreEqual(2, _model.GetEdgeCount());

            int deleteId = ids[0];
            _model.RemoveVertex(deleteId);

            Assert.AreEqual(1, _model.GetVertexCount());
            Assert.AreEqual(0, _model.GetEdgeCount());
            Assert.IsFalse(_model.GetVertexIds().Contains(deleteId));
        }

        [TestMethod]
        public void RemoveVertexWithOutEdge()
        {
            _model.NewGraph(true);
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 10);

            Assert.AreEqual(2, _model.GetVertexCount());
            Assert.AreEqual(1, _model.GetEdgeCount());

            int deleteId = ids[0];
            _model.RemoveVertex(deleteId);

            Assert.AreEqual(1, _model.GetVertexCount());
            Assert.AreEqual(0, _model.GetEdgeCount());
            Assert.IsFalse(_model.GetVertexIds().Contains(deleteId));
        }

        [TestMethod]
        public void RemoveVertexWithInEdge()
        {
            _model.NewGraph(true);
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[1], ids[0], 10);

            Assert.AreEqual(2, _model.GetVertexCount());
            Assert.AreEqual(1, _model.GetEdgeCount());

            int deleteId = ids[0];
            _model.RemoveVertex(deleteId);

            Assert.AreEqual(1, _model.GetVertexCount());
            Assert.AreEqual(0, _model.GetEdgeCount());
            Assert.IsFalse(_model.GetVertexIds().Contains(deleteId));
        }

        [TestMethod]
        public void RemoveUndirectedEdge()
        {
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 10);

            Assert.AreEqual(2, _model.GetVertexCount());
            Assert.AreEqual(2, _model.GetEdgeCount());

            _model.RemoveEdge(ids[0], ids[1]);

            Assert.AreEqual(2, _model.GetVertexCount());
            Assert.AreEqual(0, _model.GetEdgeCount());
            Assert.IsNull(_model.GetWeight(ids[0], ids[1]));
        }

        [TestMethod]
        public void RemoveDirectedEdge()
        {
            _model.NewGraph(true);
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 10);

            Assert.AreEqual(1, _model.GetEdgeCount());

            _model.RemoveEdge(ids[0], ids[1]);

            Assert.AreEqual(0, _model.GetEdgeCount());
            Assert.IsNull(_model.GetWeight(ids[0], ids[1]));
        }

        [TestMethod]
        public void RemoveUndirectedEdgeNonExistVertex()
        {
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 10);
            _model.RemoveEdge(ids[0], ids.Last() + 1);

            Assert.AreEqual(2, _model.GetEdgeCount());
        }

        [TestMethod]
        public void RemoveDirectedEdgeNonExistVertex()
        {
            _model.NewGraph(true);
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 10);
            _model.RemoveEdge(ids[0], ids.Last() + 1);

            Assert.AreEqual(1, _model.GetEdgeCount());
        }

        [TestMethod]
        public void RemoveNonExistUndirectedEdge()
        {
            _model.AddVertex();
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[2], 10);
            _model.RemoveEdge(ids[0], ids[1]);

            Assert.AreEqual(3, _model.GetVertexCount());
            Assert.AreEqual(2, _model.GetEdgeCount());
        }

        [TestMethod]
        public void RemoveNonExistDirectedEdge()
        {
            _model.NewGraph(true);
            _model.AddVertex();
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 10);
            _model.RemoveEdge(ids[1], ids[0]);

            Assert.AreEqual(3, _model.GetVertexCount());
            Assert.AreEqual(1, _model.GetEdgeCount());

            _model.RemoveEdge(ids[1], ids[2]);
            Assert.AreEqual(3, _model.GetVertexCount());
            Assert.AreEqual(1, _model.GetEdgeCount());
        }

        [TestMethod]
        public void IncementUndirectedEdgeWeight()
        {
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 10);

            _model.IncrementWeight(ids[0], ids[1]);

            Assert.AreEqual((short)11, _model.GetWeight(ids[0], ids[1]));
            Assert.AreEqual((short)11, _model.GetWeight(ids[1], ids[0]));
        }

        [TestMethod]
        public void IncementDirectedEdgeWeight()
        {
            _model.NewGraph(true);
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 10);
            _model.AddEdge(ids[1], ids[0], 10);

            _model.IncrementWeight(ids[0], ids[1]);

            Assert.AreEqual((short)11, _model.GetWeight(ids[0], ids[1]));
            Assert.AreEqual((short)10, _model.GetWeight(ids[1], ids[0]));
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void IncementNonExistEdgeWeight(bool isDirected)
        {
            _model.NewGraph(isDirected);
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.IncrementWeight(ids[0], ids[1]);

            Assert.IsNull(_model.GetWeight(ids[0], ids[1]));
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void IncementEdgeWeightNonExistVertex(bool isDirected)
        {
            _model.NewGraph(isDirected);
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.IncrementWeight(ids[0], ids.Last() + 1);

            Assert.AreEqual(0, _model.GetEdgeCount());
            Assert.IsNull(_model.GetWeight(ids[0], ids.Last() + 1));
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        [DataRow(false)]
        [DataRow(true)]
        public void IncementWeightOverFlow(bool isDirected)
        {
            _model.NewGraph(isDirected);
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 9999);

            try
            {
                _model.IncrementWeight(ids[0], ids[1]);
            }
            catch (OverflowException) 
            {
                Assert.AreEqual((short)9999, _model.GetWeight(ids[0], ids[1]));
                throw;
            }
        }

        [TestMethod]
        [DataRow((short)10)]
        [DataRow((short)9999)]
        [DataRow((short)-9999)]
        public void UpdateUndirectedEdgeWeight(short weight)
        {
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 0);

            _model.UpdateWeight(ids[0], ids[1], weight);

            Assert.AreEqual(weight, _model.GetWeight(ids[0], ids[1]));
            Assert.AreEqual(weight, _model.GetWeight(ids[1], ids[0]));
        }

        [TestMethod]
        [DataRow((short)10)]
        [DataRow((short)9999)]
        [DataRow((short)-9999)]
        public void UpdateDirectedEdgeWeight(short weight)
        {
            _model.NewGraph(true);
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 0);
            _model.AddEdge(ids[1], ids[0], 0);

            _model.UpdateWeight(ids[0], ids[1], weight);

            Assert.AreEqual(weight, _model.GetWeight(ids[0], ids[1]));
            Assert.AreEqual((short)0, _model.GetWeight(ids[1], ids[0]));
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void UpdateNonExistEdgeWeight(bool isDirected)
        {
            _model.NewGraph(isDirected);
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.UpdateWeight(ids[0], ids[1], 10);

            Assert.IsNull(_model.GetWeight(ids[0], ids[1]));
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void UpdateEdgeWeightNonExistVertex(bool isDirected)
        {
            _model.NewGraph(isDirected);
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.UpdateWeight(ids[0], ids.Last() + 1, 10);

            Assert.AreEqual(0, _model.GetEdgeCount());
            Assert.IsNull(_model.GetWeight(ids[0], ids.Last() + 1));
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        [DataRow((short)10000, false)]
        [DataRow((short)-10000, false)]
        [DataRow((short)10000, true)]
        [DataRow((short)-10000, true)]
        public void UpdateWeightOverFlow(short weight, bool isDirected)
        {
            _model.NewGraph(isDirected);
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 0);

            try
            {
                _model.UpdateWeight(ids[0], ids[1], weight);
            }
            catch (OverflowException)
            {
                Assert.AreEqual((short)0, _model.GetWeight(ids[0], ids[1]));
                throw;
            }
        }

        [TestMethod]
        public async Task LoadAsync()
        {
            await _model.LoadAsync(String.Empty);

            Assert.AreEqual(_mockedGraph.IsDirected, _model.IsDirected);
            Assert.AreEqual(_mockedGraph.VertexCount, _model.GetVertexCount());
            Assert.AreEqual(_mockedGraph.EdgeCount, _model.GetEdgeCount());

            List<int> actualIds = _model.GetVertexIds();
            List<int> expectedIds = _mockedGraph.VertexIds;

            for(int i = 0; i < _mockedGraph.VertexCount; ++i)
            {
                Assert.AreEqual(expectedIds[i], actualIds[i]);
            }

            for(int i = 0; i < _mockedGraph.VertexCount; ++i)
            {
                for(int j = 0; j < _mockedGraph.VertexCount; ++j)
                {
                    Vertex from = _mockedGraph.Vertices[i];
                    Vertex to = _mockedGraph.Vertices[j];
                    
                    Assert.AreEqual(_mockedGraph.GetWeight(from, to), _model.GetWeight(from.Id, to.Id));
                }
            }
        }

        #endregion

        #region Algorithm tests

        [TestMethod]
        public void InitAlgorithm()
        {
            FillGraph();

            _model.InitAlgorithm();

            Assert.IsTrue(_model.IsAlgorthmInitialized);
            Assert.IsTrue(_model.HasNextStep);
            Assert.IsFalse(_model.HasPreviousStep);
            Assert.AreEqual(0, _model.K);
            Assert.IsNull(_model.PrevK);
        }

        [TestMethod]
        public void CancelAlgorithm()
        {
            FillGraph();

            _model.InitAlgorithm();
            _model.CancelAlgorithm();

            Assert.IsFalse(_model.IsAlgorthmInitialized);
            Assert.IsFalse(_model.HasNextStep);
            Assert.IsFalse(_model.HasPreviousStep);
            Assert.IsNull(_model.K);
            Assert.IsNull(_model.PrevK);
        }

        [TestMethod]
        public void StepAlgoirthm()
        {
            FillGraph();
            _model.InitAlgorithm();

            List<int> ids = _model.GetVertexIds();
            int i = 0;

            while (_model.HasNextStep)
            {
                _model.StepAlgorithm();
                Assert.AreEqual(ids[i], _model.K);
                if(i > 0)
                {
                    Assert.AreEqual(ids[i - 1], _model.PrevK);
                }
                else
                {
                    Assert.AreEqual(0, _model.PrevK);
                }
                ++i;
            }

            Assert.IsTrue(_model.HasPreviousStep);
        }

        [TestMethod]
        public void StepUnInitializedAlgoirthm()
        {
            FillGraph();

            List<int> ids = _model.GetVertexIds();
            int i = 0;

            while (_model.HasNextStep)
            {
                _model.StepAlgorithm();
                Assert.AreEqual(ids[i], _model.K);
                if (i > 0)
                {
                    Assert.AreEqual(ids[i - 1], _model.PrevK);
                }
                else
                {
                    Assert.AreEqual(0, _model.PrevK);
                }
                ++i;
            }

            Assert.IsFalse(_model.IsAlgorthmInitialized);
            Assert.IsFalse(_model.HasNextStep);
            Assert.IsFalse(_model.HasPreviousStep);
            Assert.IsNull(_model.K);
            Assert.IsNull(_model.PrevK);
        }

        [TestMethod]
        public void StepAlgorithmBack()
        {
            FillGraph();
            _model.InitAlgorithm();

            List<int> ids = _model.GetVertexIds();
            int i = 0;

            while (_model.HasNextStep)
            {
                _model.StepAlgorithm();
                ++i;
            }

            while (_model.HasPreviousStep)
            {
                --i;
                _model.StepAlgorithmBack();
                Assert.AreEqual(ids[i-1], _model.K);
                if(i > 1)
                {
                    Assert.AreEqual(ids[i - 2], _model.PrevK);
                }
                else
                {
                    Assert.AreEqual(0, _model.PrevK);
                }

            }

            Assert.IsTrue(_model.HasNextStep);
        }

        [TestMethod]
        public void StepAlgoirthmNegCycle()
        {
            FillGraphNegCycle();
            _model.InitAlgorithm();

            List<int> ids = _model.GetVertexIds();
            int i = 0;

            while (_model.HasNextStep)
            {
                _model.StepAlgorithm();
                Assert.AreEqual(ids[i], _model.K);
                if (i > 0)
                {
                    Assert.AreEqual(ids[i - 1], _model.PrevK);
                }
                else
                {
                    Assert.AreEqual(0, _model.PrevK);
                }
                ++i;
            }

            Assert.IsTrue(_model.HasPreviousStep);
        }

        [TestMethod]
        public void GetRoute()
        {
            FillGraph();
            _model.InitAlgorithm();

            _model.StepAlgorithm();

            _model.StepAlgorithm();

            List<int> ids = _model.GetVertexIds();

            _model.GetRoute(ids[0], ids[2], false);
            _model.GetRoute(ids[0], ids.Last() + 1, false);
        }

        #endregion

        #region Event handlers

        private void Model_AlgorithmInitialized(object? sender, AlgorithmInitEventArgs e)
        {
            AlgorithmData? data = _model.GetAlgorithmData();

            Assert.IsNotNull(data);
            Assert.AreEqual(0, data.ChangesD.Count);
            Assert.AreEqual(0, data.ChangesPi.Count);
            Assert.AreEqual(data.D.GetLength(0), e.D.GetLength(0));
            Assert.AreEqual(data.D.GetLength(1), e.D.GetLength(1));
            Assert.AreEqual(data.Pi.GetLength(0), e.Pi.GetLength(0));
            Assert.AreEqual(data.Pi.GetLength(1), e.Pi.GetLength(1));

            for (int i = 0; i < data.D.GetLength(0); ++i)
            {
                for (int j = 0; j < data.D.GetLength(1); ++j)
                {
                    Assert.AreEqual(data.D[i, j], e.D[i, j]);
                    Assert.AreEqual(data.Pi[i, j], e.Pi[i, j]);
                }
            }
        }

        private void Model_AlgorithmStepped(object? sender, AlgorithmSteppedEventArgs e)
        {
            AlgorithmData? data = _model.GetAlgorithmData();

            Assert.IsTrue(data != null);
            Assert.AreEqual(data.ChangesD.Count, e.ChangeD.Count);
            Assert.AreEqual(data.ChangesPi.Count, e.ChangePi.Count);

            foreach(ChangeValue c in e.ChangeD)
            {
                int i = c.I;
                int j = c.J;
                Assert.IsTrue(data.ChangesD.Contains(c));
                Assert.AreEqual(data.D[i, j], c.Value);
            }

            foreach (ChangeValue c in e.ChangePi)
            {
                int i = c.I;
                int j = c.J;
                Assert.IsTrue(data.ChangesPi.Contains(c));
                Assert.AreEqual(data.Pi[i, j], c.Value);
            }

            foreach (ChangeValue c in e.ChangePrevD)
            {
                int i = c.I;
                int j = c.J;
                Assert.AreEqual(data.PrevD[i, j], c.Value);
            }

            foreach (ChangeValue c in e.ChangePrevPi)
            {
                int i = c.I;
                int j = c.J;
                Assert.AreEqual(data.PrevPi[i, j], c.Value);
            }
        }

        private void Model_AlogorithmSteppedBack(object? sender, AlgorithmSteppedEventArgs e)
        {
            AlgorithmData? data = _model.GetAlgorithmData();

            Assert.IsTrue(data != null);
            Assert.AreEqual(data.ChangesD.Count, e.ChangePrevD.Count);
            Assert.AreEqual(data.ChangesPi.Count, e.ChangePrevPi.Count);

            foreach (ChangeValue c in e.ChangeD)
            {
                int i = c.I;
                int j = c.J;
                Assert.AreEqual(data.D[i, j], c.Value);
            }

            foreach (ChangeValue c in e.ChangePi)
            {
                int i = c.I;
                int j = c.J;
                Assert.AreEqual(data.Pi[i, j], c.Value);
            }

            foreach (ChangeValue c in e.ChangePrevD)
            {
                int i = c.I;
                int j = c.J;
                Assert.IsTrue(data.ChangesD.Contains(c));
                Assert.AreEqual(data.PrevD[i, j], c.Value);
            }

            foreach (ChangeValue c in e.ChangePrevPi)
            {
                int i = c.I;
                int j = c.J;
                Assert.IsTrue(data.ChangesPi.Contains(c));
                Assert.AreEqual(data.PrevPi[i, j], c.Value);
            }
        }

        private void Model_RouteCreated(object? sender, RouteEventArgs e)
        {
            List<int> ids = _model.GetVertexIds();

            List<int> expectedResult = new List<int> { ids[0], ids[1], ids[2] };

            Assert.AreEqual(expectedResult.Count, e.Route.Count);

            for(int i = 0; i < expectedResult.Count; ++i)
            {
                Assert.AreEqual(expectedResult[i], e.Route[i]);
            }
        }

        private void Model_AlgorithmEnded(object? sender, EventArgs e)
        {
            Assert.IsFalse(_model.HasNextStep);
        }

        private void Model_NegativeCycleFound(object? sender, RouteEventArgs e)
        {
            Assert.IsFalse(_model.HasNextStep);

            List<int> ids = _model.GetVertexIds();

            List<int> expectedResult = new List<int> { ids[0], ids[1], ids[2], ids[4] };

            Assert.AreEqual(expectedResult.Count, e.Route.Count);

            for (int i = 0; i < expectedResult.Count; ++i)
            {
                Assert.AreEqual(expectedResult[i], e.Route[i]);
            }
        }

        #endregion

        #region Private methods

        private void FillGraph()
        {
            _model.AddVertex();
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.RemoveVertex(ids.Last());

            _model.AddVertex();
            _model.AddVertex();
            _model.AddVertex();

            ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 10);
            _model.AddEdge(ids[0], ids[3], 5);
            _model.AddEdge(ids[1], ids[2], 55);
            _model.AddEdge(ids[1], ids[4], 20);
            _model.AddEdge(ids[2], ids[3], 10);
            _model.AddEdge(ids[3], ids[4], 0);
        }

        private void FillGraphNegCycle()
        {
            _model.NewGraph(true);

            _model.AddVertex();
            _model.AddVertex();
            _model.AddVertex();

            List<int> ids = _model.GetVertexIds();

            _model.RemoveVertex(ids.Last());

            _model.AddVertex();
            _model.AddVertex();
            _model.AddVertex();

            ids = _model.GetVertexIds();

            _model.AddEdge(ids[0], ids[1], 2);
            _model.AddEdge(ids[1], ids[2], 0);
            _model.AddEdge(ids[2], ids[4], -10);
            _model.AddEdge(ids[4], ids[0], 3);
            _model.AddEdge(ids[2], ids[3], 4);
            _model.AddEdge(ids[3], ids[4], -1);
        }

        #endregion
    }
}