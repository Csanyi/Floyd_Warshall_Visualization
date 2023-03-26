using Floyd_Warshall_Model;
using Floyd_Warshall.ViewModel.Commands.GraphCanvasCommands;
using Floyd_Warshall_Model.Graph;
using Floyd_Warshall_Model.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Floyd_Warshall_Model.Events;
using System.Windows;

namespace Floyd_Warshall.ViewModel.GraphComponents
{
    public class GraphCanvasViewModel : ViewModelBase
    {
        #region Fields

        private readonly GraphModel _graphModel;

        public const int maxVertexCount = 14;

        private int _vertexId;
        private int _edgeId;
        public VertexViewModel? _selectedVertex;
        private EdgeViewModelBase? _selectedEdge;
        private bool _canvasEnabled;

        #endregion

        #region Properties

        public int GetVertexId { get { return ++_vertexId; } }

        public int GetEdgeId { get { return ++_edgeId; } }

        public VertexViewModel? SelectedVertex 
        { 
            get { return _selectedVertex; } 
            set 
            {
                if(_selectedVertex != null)
                {
                    _selectedVertex.IsSelected = false;
                }

                _selectedVertex = value;

                if (_selectedVertex != null)
                {
                    _selectedVertex.IsSelected = true;
                }
            } 
        }

        public EdgeViewModelBase? SelectedEdge
        {
            get { return _selectedEdge; }
            set
            {
                if(_selectedEdge != null)
                {
                    _selectedEdge.IsSelected = false;
                }

                _selectedEdge = value;

                if (_selectedEdge != null)
                {
                    _selectedEdge.IsSelected = true;
                }

                OnPropertyChanged(nameof(IsEdgeSelected));
                OnPropertyChanged();
            }
        }

        public bool IsEdgeSelected { get { return SelectedEdge != null; } }

        public double MouseX { get; set; }
        public double MouseY { get; set; }

        public bool CanvasEnabled
        {
            get => _canvasEnabled;
            set
            {
                _canvasEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool MaxVertexCountReached { get { return _graphModel.GetVertexCount() >= maxVertexCount; } }

        public List<VertexViewModel> Verteces { get; set; }

        public List<EdgeViewModelBase> Edges { get; set; }

        public ObservableCollection<GraphComponentViewModel> Views { get; set; }

        public ICommand CanvasClickCommand { get; private set; }

        #endregion

        #region Constructors

        public GraphCanvasViewModel(GraphModel graphModel)
        {
            _graphModel = graphModel;

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

            Verteces = new List<VertexViewModel>();
            Edges = new List<EdgeViewModelBase>();
            Views = new ObservableCollection<GraphComponentViewModel>();

            _vertexId = 0;
            _edgeId = 0;
            _selectedVertex = null;
            _selectedEdge = null;
            _canvasEnabled = true;
        }

        #endregion

        #region Public methods

        public IEnumerable<VertexLocation> GetLocations()
        {
            return Verteces.Select(v => new VertexLocation(v.Vertex, v.CanvasX, v.CanvasY));
        }

        #endregion

        #region Private methods

        private void Init()
        {
            Verteces.Clear();
            Edges.Clear();
            Views.Clear();

            _vertexId = 0;
            _edgeId = 0;

            SelectedEdge = null;
            SelectedVertex = null;

            OnPropertyChanged(nameof(MaxVertexCountReached));
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

        private void SelectRoute(List<int> route, bool isNegCycle)
        {
            Edges.ForEach(e => e.IsSelected = false);
            Verteces.ForEach(v => v.IsSelected = false);

            for(int i = 0; i < route.Count; ++i)
            {
                VertexViewModel? v = Verteces.FirstOrDefault(v => v.Id == route[i]);

                if (v == null) { continue; }
                if (isNegCycle)
                {
                    v.InNegCycle = true;
                }
                else
                {
                    v.IsSelected = true;
                }

                EdgeViewModelBase? edgevm = null;

                if (i < route.Count - 1 || isNegCycle)
                {
                    int ind = (i + 1) % route.Count;

                    if (_graphModel.IsDirected)
                    {
                        edgevm = v.Edges.FirstOrDefault(edge => edge.To.Id == route[ind]);
                    }
                    else
                    {
                        edgevm = v.Edges.FirstOrDefault(edge => edge.From.Id == route[ind] || edge.To.Id == route[ind]);
                    }

                    if (edgevm != null)
                    {
                        edgevm.IsSelected = true;
                    }
                }
            }
        }

        private void ClearSelections()
        {
            Verteces.ForEach(v =>
            {
                v.IsSelected = false; 
                v.InNegCycle = false;
            });
            Edges.ForEach(e => e.IsSelected = false);
        }

        #endregion

        #region Model event handlers

        private void Model_NewEmptyGraph(object? sender, EventArgs e)
        {
            Init();
        }

        private void Model_GraphLoaded(object? sender, GraphLoadedEventArgs e)
        {
            if(e.VertexLocations.Count() > maxVertexCount) 
            {
                _graphModel.NewGraph(false);
                throw new GraphDataException();
            }

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
            SelectedVertex = null;
            SelectedEdge = null;
            CanvasEnabled = false;
        }

        private void Model_AlgorithmStepped(object? sender, EventArgs e)
        {
            ClearSelections();
        }

        private void Model_AlgorithmStopped(object? sender, EventArgs e)
        {
            ClearSelections();

            CanvasEnabled = true;
        }

        private void Model_NegativeCycleFound(object? sender, RouteEventArgs e)
        {
            SelectRoute(e.Route, true);
        }

        private void Model_VertexCntChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(MaxVertexCountReached));
        }

        private void Model_RouteCreated(object? sender, RouteEventArgs e)
        {
            SelectRoute(e.Route, false);
        }

        #endregion
    }
}
