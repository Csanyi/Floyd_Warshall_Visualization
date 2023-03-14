using Floyd_Warshall_Model;
using Floyd_Warshall.ViewModel.Commands;
using Floyd_Warshall_Model.Graph;
using Floyd_Warshall_Model.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Floyd_Warshall_Model.Events;

namespace Floyd_Warshall.ViewModel.GraphComponents
{
    public class GraphCanvasViewModel : ViewModelBase
    {
        private readonly GraphModel _graphModel;

        private int _vertexId = 0;
        public int GetVertexId { get { return ++_vertexId; } }

        private int _edgeId = 0;
        public int GetEdgeId { get { return ++_edgeId; } }

        public VertexViewModel? _selectedVertex = null;
        public VertexViewModel? SelectedVertex { get { return _selectedVertex; } set { _selectedVertex = value; } }

        private EdgeViewModelBase? _selectedEdge = null;
        public EdgeViewModelBase? SelectedEdge
        {
            get { return _selectedEdge; }
            set
            {
                _selectedEdge = value;
                OnPropertyChanged(nameof(IsEdgeSelected));
                OnPropertyChanged();
            }
        }

        public bool IsEdgeSelected { get { return SelectedEdge != null; } }

        public double MouseX { get; set; }
        public double MouseY { get; set; }

        private bool _canvasEnabled = true;
        public bool CanvasEnabled
        {
            get => _canvasEnabled;
            set
            {
                _canvasEnabled = value;
                OnPropertyChanged();
            }
        }

        public const int maxVertexCount = 20;

        public bool MaxVertexCountReached { get { return _graphModel.GetVertexCount() >= maxVertexCount; } }

        public List<VertexViewModel> Verteces { get; set; }

        public List<EdgeViewModelBase> Edges { get; set; }

        public ObservableCollection<GraphComponentViewModel> Views { get; set; }

        public ICommand CanvasClickCommand { get; private set; }

        public GraphCanvasViewModel(GraphModel graphModel)
        {
            _graphModel = graphModel;

            Verteces = new List<VertexViewModel>();
            Edges = new List<EdgeViewModelBase>();
            Views = new ObservableCollection<GraphComponentViewModel>();

            CanvasClickCommand = new CanvasClickCommand(this, _graphModel);

            _graphModel.NewEmptyGraph += Model_NewEmptyGraph;
            _graphModel.GraphLoaded += Model_GraphLoaded;
            _graphModel.AlgorithmStarted += Model_AlgorithmStarted;
            _graphModel.AlgorithmStopped += Model_AlgorithmStopped;
            _graphModel.AlgorithmStepped += Model_AlgorithmStepped;
            _graphModel.NegativeCycleFound += Model_NegativeCycleFound;
            _graphModel.VertexAdded += Model_VertexCntChanged;
            _graphModel.VertexRemoved += Model_VertexCntChanged;
            _graphModel.RouteCreated += Model_RouteCreated;
        }

        public IEnumerable<VertexLocation> GetLocations() 
                        => Verteces.Select(v => new VertexLocation(v.Vertex,v.CanvasX, v.CanvasY));


        private void Init()
        {
            Verteces.Clear();
            Edges.Clear();
            Views.Clear();

            _vertexId = 0;
            _edgeId = 0;

            _selectedEdge = null;
            _selectedVertex = null;
        }

        private void CreateDirectedEdges()
        {
            foreach (Edge edge in _graphModel.GetEdges())
            {
                VertexViewModel from = Verteces.Single(v => v.Vertex == edge.From);
                VertexViewModel to = Verteces.Single(v => (v.Vertex == edge.To));

                EdgeViewModelBase edgevm = new DirectedEdgeViewModel(GetEdgeId, _graphModel, from, to)
                {
                    Weight = edge.Weight,
                    IsSelected = false,
                    LeftClickCommand = new EdgeLeftClickCommand(this),
                    RightClickCommand = new EdgeRightClickCommand(this, _graphModel),
                };

                Edges.Add(edgevm);
                from.Edges.Add(edgevm);
                to.Edges.Add(edgevm);
                Views.Add(edgevm);
            }
        }

        private void CreateUndirectedEdges()
        {
            foreach (Edge edge in _graphModel.GetEdges())
            {
                VertexViewModel from = Verteces.Single(v => v.Vertex == edge.From);
                VertexViewModel to = Verteces.Single(v => (v.Vertex == edge.To));

                if (!from.Edges.Exists(e => (e.From == from && e.To == to) || (e.From == to && e.To == from)))
                {
                    EdgeViewModelBase edgevm = new EdgeViewModel(GetEdgeId, _graphModel, from, to)
                    {
                        Weight = edge.Weight,
                        IsSelected = false,
                        LeftClickCommand = new EdgeLeftClickCommand(this),
                        RightClickCommand = new EdgeRightClickCommand(this, _graphModel),
                    };

                    Edges.Add(edgevm);
                    from.Edges.Add(edgevm);
                    to.Edges.Add(edgevm);
                    Views.Add(edgevm);
                }
            }
        }

        private void ClearSelections()
        {
            Verteces.ForEach(v => v.IsSelected = false);
            Edges.ForEach(e => e.IsSelected = false);
        }


        #region Model event handlers

        private void Model_NewEmptyGraph(object? sender, EventArgs e) => Init();

        private void Model_GraphLoaded(object? sender, GraphLoadedEventArgs e)
        {
            Init();

            foreach (var v in e.VertexLocations)
            {
                if(v.Vertex.Id >_vertexId) { _vertexId = v.Vertex.Id; }

                VertexViewModel vertex = new VertexViewModel(v.Vertex)
                {
                    CanvasX = v.X,
                    CanvasY = v.Y,
                    IsSelected = false,
                    RightClickCommand = new VertexRightClickCommand(this, _graphModel),
                    LeftClickCommand = new VertexLeftClickCommand(this, _graphModel),
                };

                Verteces.Add(vertex);
                Views.Add(vertex);
            }

            if(_graphModel.IsDirected)
            {
                CreateDirectedEdges();
            } else
            {
                CreateUndirectedEdges();
            }
        }

        private void Model_AlgorithmStarted(object? sender, AlgorithmEventArgs e)
        {
            if(SelectedVertex != null)
            {
                SelectedVertex.IsSelected = false;
                SelectedVertex = null;
            }

            if (SelectedEdge != null)
            {
                SelectedEdge.IsSelected = false;
                SelectedEdge = null;
            }

            CanvasEnabled = false;
        }

        private void Model_AlgorithmStepped(object? sender, EventArgs e)
        {
            ClearSelections();
        }

        private void Model_AlgorithmStopped(object? sender, EventArgs e)
        {
            if(SelectedVertex != null)
            {
                SelectedVertex.InNegCycle = false;
                SelectedVertex = null;
            }

            ClearSelections();

            CanvasEnabled = true;
        }

        private void Model_NegativeCycleFound(object? sender, int e)
        {
            VertexViewModel? v = Verteces.FirstOrDefault(v => v.Vertex.Id == e);

            if(v != null)
            {
                v.InNegCycle = true;
                SelectedVertex = v;
            }
        }

        private void Model_VertexCntChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(MaxVertexCountReached));
        }

        private void Model_RouteCreated(object? sender, RouteEventArgs e)
        {
            Edges.ForEach(e => e.IsSelected = false);

            foreach(VertexViewModel v in Verteces)
            {
                if(e.Route.Contains(v.Id))
                {
                    v.IsSelected = true;
                    int ind = e.Route.FindIndex(x => x == v.Id);
                    if(ind < e.Route.Count - 1)
                    {
                        EdgeViewModelBase? edgevm = null;

                        if (_graphModel.IsDirected)
                        {
                            edgevm = v.Edges.FirstOrDefault(edge => edge.To.Id == e.Route[ind + 1]);
                        }
                        else
                        {
                            edgevm = v.Edges.FirstOrDefault(edge => edge.From.Id == e.Route[ind + 1] || edge.To.Id == e.Route[ind + 1]);
                        }

                        if(edgevm != null)
                        {
                            edgevm.IsSelected = true;
                        }
                    }
                }
                else
                {
                    v.IsSelected = false;
                }
            }
        }

        #endregion
    }
}
