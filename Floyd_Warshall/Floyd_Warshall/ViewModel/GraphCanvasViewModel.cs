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
using Accessibility;

namespace Floyd_Warshall.ViewModel.GraphComponents
{
    public class GraphCanvasViewModel : ViewModelBase
    {
        #region Fields

        private readonly GraphModel _graphModel;

        public VertexViewModel? _selectedVertex;
        private EdgeViewModelBase? _selectedEdge;
        private bool _canvasEnabled;
        private bool _hasInputError;
        private string _weightText;

        #endregion

        #region Properties

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
                    _selectedEdge.EdgeUpdated -= SelectedEdge_EdgeUpdated;
                }

                _selectedEdge = value;

                if (_selectedEdge != null)
                {
                    WeightText = _selectedEdge.Weight.ToString();
                    _selectedEdge.IsSelected = true;
                    _selectedEdge.EdgeUpdated += SelectedEdge_EdgeUpdated;
                }
                OnPropertyChanged(nameof(IsEdgeSelected));
                OnPropertyChanged();
            }
        }

        public bool IsEdgeSelected { get { return SelectedEdge != null; } }

        public string WeightText
        {
            get { return _weightText; }
            set
            {
                _weightText = value;
                OnPropertyChanged();
            }
        }

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

        public bool MaxVertexCountReached { get { return _graphModel.GetVertexCount() >= GraphModel.maxVertexCount; } }

        public bool HasInputError
        {
            get { return _hasInputError; }
            set
            {
                _hasInputError = value;
                OnPropertyChanged();
            }
        }

        public List<VertexViewModel> Verteces { get; set; }

        public List<EdgeViewModelBase> Edges { get; set; }

        public ObservableCollection<GraphComponentViewModel> Views { get; set; }

        public ICommand CanvasClickCommand { get; private set; }
        public ICommand SubmitCommand { get; private set; }

        #endregion

        #region Constructors

        public GraphCanvasViewModel(GraphModel graphModel)
        {
            _graphModel = graphModel;

            CanvasClickCommand = new CanvasClickCommand(this, _graphModel);
            SubmitCommand = new SubmitCommand(this, _graphModel);

            _graphModel.NewEmptyGraph += Model_NewEmptyGraph;
            _graphModel.GraphLoaded += Model_GraphLoaded;
            _graphModel.AlgorithmStarted += Model_AlgorithmStarted;
            _graphModel.AlgorithmStopped += Model_AlgorithmStopped;
            _graphModel.AlgorithmStepped += Model_AlgorithmStepped;
            _graphModel.NegativeCycleFound += Model_NegativeCycleFound;
            _graphModel.VertexAdded += Model_VertexAdded;
            _graphModel.DirectedEdgeAdded += Model_DirectedEdgeAdded;
            _graphModel.UndirectedEdgeAdded += Model_UndirectedEdgeAdded;
            _graphModel.VertexRemoved += Model_VertexCntChanged;
            _graphModel.RouteCreated += Model_RouteCreated;

            Verteces = new List<VertexViewModel>();
            Edges = new List<EdgeViewModelBase>();
            Views = new ObservableCollection<GraphComponentViewModel>();

            _selectedVertex = null;
            _selectedEdge = null;
            _canvasEnabled = true;
            _hasInputError = false;
            _weightText = "";
        }

        #endregion

        #region Public methods

        public IEnumerable<VertexLocation> GetLocations()
        {
            return Verteces.Select(v => new VertexLocation(v.Id, v.CanvasX, v.CanvasY));
        }

        #endregion

        #region Private methods

        private void Init()
        {
            Verteces.Clear();
            Edges.Clear();
            Views.Clear();

            SelectedEdge = null;
            SelectedVertex = null;

            OnPropertyChanged(nameof(MaxVertexCountReached));
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

        private void Model_VertexAdded(object? sender, VertexAddedEventArgs e)
        {
            OnPropertyChanged(nameof(MaxVertexCountReached));

            VertexViewModel vertex = new VertexViewModel(e.Id)
            {
                CanvasX = MouseX - VertexViewModel.Size / 2,
                CanvasY = MouseY - VertexViewModel.Size / 2,
                IsSelected = false,
                RightClickCommand = new VertexRightClickCommand(this, _graphModel),
                LeftClickCommand = new VertexLeftClickCommand(this, _graphModel),
            };

            Verteces.Add(vertex);
            Views.Add(vertex);
        }

        private void Model_DirectedEdgeAdded(object? sender, EdgeAddedEventArgs e)
        {
            VertexViewModel from = Verteces.First(v => v.Id == e.From);
            VertexViewModel to = Verteces.First(v => v.Id == e.To);

            DirectedEdgeViewModel edge = new DirectedEdgeViewModel(e.Id, _graphModel, from, to)
            {
                IsSelected = false,
                LeftClickCommand = new EdgeLeftClickCommand(this),
                RightClickCommand = new EdgeRightClickCommand(this, _graphModel),
            };

            Edges.Add(edge);
            Views.Add(edge);

            from.Edges.Add(edge);
            to.Edges.Add(edge);
        }

        private void Model_UndirectedEdgeAdded(object? sender, EdgeAddedEventArgs e)
        {
            VertexViewModel from = Verteces.First(v => v.Id == e.From);
            VertexViewModel to = Verteces.First(v => v.Id == e.To);

            EdgeViewModel edge = new EdgeViewModel(e.Id, _graphModel, from, to)
            {
                IsSelected = false,
                LeftClickCommand = new EdgeLeftClickCommand(this),
                RightClickCommand = new EdgeRightClickCommand(this, _graphModel),
            };

            Edges.Add(edge);
            Views.Add(edge);

            from.Edges.Add(edge);
            to.Edges.Add(edge);
        }

        private void Model_GraphLoaded(object? sender, GraphLoadedEventArgs e)
        {
            Init();

            foreach (var v in e.VertexLocations)
            {
                VertexViewModel vertex = new VertexViewModel(v.Id)
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
        }

        private void Model_AlgorithmStarted(object? sender, EventArgs e)
        {
            HasInputError = false;
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

        #region Edge event handlers

        private void SelectedEdge_EdgeUpdated(object? sender, EventArgs e)
        {
            if(SelectedEdge != null)
            {
                WeightText = SelectedEdge.Weight.ToString();
            }
        }

        #endregion
    }
}
