using Floyd_Warshall.ViewModel.Commands.GraphCanvasCommands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Floyd_Warshall_Model.Model;
using Floyd_Warshall_Model.Model.Events;

namespace Floyd_Warshall.ViewModel.GraphComponents
{
	/// <summary>
	/// Type of the graph canvas viewmodel
	/// </summary>
	public class GraphCanvasViewModel : ViewModelBase
    {
        #region Fields

        private readonly GraphModel _graphModel;  // the graph model

        public VertexViewModel? _selectedVertex;  // the selected vertex viewmodel
        private EdgeViewModelBase? _selectedEdge; // the selected edge viewmodel
        private bool _canvasEnabled;              // indicates whether the canvas is enabled
        private bool _hasInputError;              // indicates whether there is an input error
        private string _weightText;               // the weight of the selected edge

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the selected vertex viewmodel
        /// </summary>
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

        /// <summary>
        /// Gets or sets the selected edge viewmodel
        /// </summary>
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

        /// <summary>
        /// Indicates whether an edge is selected
        /// </summary>
        public bool IsEdgeSelected { get { return SelectedEdge != null; } }

        /// <summary>
        /// Gets or sets the weightText field
        /// </summary>
        public string WeightText
        {
            get { return _weightText; }
            set
            {
                _weightText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the mouse'a x coord
        /// </summary>
        public double MouseX { get; set; }

        /// <summary>
        /// Gets or sets the mouse's y coord
        /// </summary>
        public double MouseY { get; set; }

        /// <summary>
        /// Gets or sets the canvasEnabled field
        /// </summary>
        public bool CanvasEnabled
        {
            get { return _canvasEnabled; }
            set
            {
                _canvasEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicates whether the vertex count has reached the maximum
        /// </summary>
        public bool MaxVertexCountReached { get { return _graphModel.GetVertexCount() >= GraphModel.MaxVertexCount; } }

        /// <summary>
        /// Indicates whether there is an input error
        /// </summary>
        public bool HasInputError
        {
            get { return _hasInputError; }
            set
            {
                _hasInputError = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the list of vertices
        /// </summary>
        public ICollection<VertexViewModel> Verteces { get; private set; }

        /// <summary>
        /// Gets or sets the list of edges
        /// </summary>
        public ICollection<EdgeViewModelBase> Edges { get; private set; }

        /// <summary>
        /// Gets or sets the list of graph components
        /// </summary>
        public ObservableCollection<GraphComponentViewModelBase> GraphComponents { get; private set; }

        /// <summary>
        /// Gets or sets tha canvas click command
        /// </summary>
        public ICommand CanvasClickCommand { get; private set; }

        /// <summary>
        /// Gets or sets the submit command
        /// </summary>
        public ICommand SubmitCommand { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the graph canvas viewmodel
        /// </summary>
        /// <param name="graphModel">The graph model</param>
        public GraphCanvasViewModel(GraphModel graphModel)
        {
            _graphModel = graphModel;

            CanvasClickCommand = new CanvasClickCommand(this, _graphModel);
            SubmitCommand = new SubmitCommand(this, _graphModel);

            _graphModel.NewGraphCreated += Model_NewGraphCreated;
            _graphModel.GraphLoaded += Model_GraphLoaded;
            _graphModel.AlgorithmInitialized += Model_AlgorithmInitialized;
            _graphModel.AlgorithmCancelled += Model_AlgorithmCancelled;
            _graphModel.AlgorithmStepped += Model_AlgorithmStepped;
            _graphModel.AlgorithmSteppedBack += Model_AlgorithmStepped;
            _graphModel.NegativeCycleFound += Model_NegativeCycleFound;
            _graphModel.VertexAdded += Model_VertexAdded;
            _graphModel.DirectedEdgeAdded += Model_DirectedEdgeAdded;
            _graphModel.UndirectedEdgeAdded += Model_UndirectedEdgeAdded;
            _graphModel.VertexRemoved += Model_VertexRemoved;
            _graphModel.RouteCreated += Model_RouteCreated;

            Verteces = new HashSet<VertexViewModel>();
            Edges = new HashSet<EdgeViewModelBase>();
            GraphComponents = new ObservableCollection<GraphComponentViewModelBase>();

            _selectedVertex = null;
            _selectedEdge = null;
            _canvasEnabled = true;
            _hasInputError = false;
            _weightText = "";
        }

        #endregion

        #region Public methods

        /// <returns>The location of the vertices</returns>
        public IEnumerable<VertexLocation> GetLocations()
        {
            return Verteces.Select(v => new VertexLocation(v.Id, v.GetX(), v.GetY()));
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Resets the values
        /// </summary>
        private void Init()
        {
            Verteces.Clear();
            Edges.Clear();
            GraphComponents.Clear();

            SelectedEdge = null;
            SelectedVertex = null;

            HasInputError = false;

            OnPropertyChanged(nameof(MaxVertexCountReached));
        }

        /// <summary>
        /// Selects the given route
        /// </summary>
        /// <param name="route">The route</param>
        /// <param name="isNegCycle">Select as negative cycle if true, otherwise selects normally</param>
        private void SelectRoute(List<int> route, bool isNegCycle)
        {
            ClearSelections();

            for (int i = 0; i < route.Count; ++i)
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

        /// <summary>
        /// Clears the selections
        /// </summary>
        private void ClearSelections()
        {
            foreach(VertexViewModel v in Verteces)
            {
                v.IsSelected = false;
                v.InNegCycle = false;
            }
            foreach (EdgeViewModelBase e in Edges)
            {
                e.IsSelected = false;
            }
        }

        #endregion

        #region Model event handlers

        private void Model_NewGraphCreated(object? sender, EventArgs e)
        {
            Init();
        }

        private void Model_VertexAdded(object? sender, VertexAddedEventArgs e)
        {
            OnPropertyChanged(nameof(MaxVertexCountReached));

            VertexViewModel vertex = new VertexViewModel(e.Id)
            {
                CanvasX = MouseX,
                CanvasY = MouseY,
                IsSelected = false,
                RightClickCommand = new VertexRightClickCommand(this, _graphModel),
                LeftClickCommand = new VertexLeftClickCommand(this, _graphModel),
            };

            Verteces.Add(vertex);
            GraphComponents.Add(vertex);
        }

        private void Model_DirectedEdgeAdded(object? sender, EdgeAddedEventArgs e)
        {
            VertexViewModel from = Verteces.First(v => v.Id == e.From);
            VertexViewModel to = Verteces.First(v => v.Id == e.To);

            EdgeViewModelBase edge = new DirectedEdgeViewModel(e.Id, _graphModel, from, to)
            {
                IsSelected = false,
                LeftClickCommand = new EdgeLeftClickCommand(this),
                RightClickCommand = new EdgeRightClickCommand(this, _graphModel),
            };

            Edges.Add(edge);
            GraphComponents.Add(edge);

            from.Edges.Add(edge);
            to.Edges.Add(edge);
        }

        private void Model_UndirectedEdgeAdded(object? sender, EdgeAddedEventArgs e)
        {
            VertexViewModel from = Verteces.First(v => v.Id == e.From);
            VertexViewModel to = Verteces.First(v => v.Id == e.To);

            EdgeViewModelBase edge = new UndirectedEdgeViewModel(e.Id, _graphModel, from, to)
            {
                IsSelected = false,
                LeftClickCommand = new EdgeLeftClickCommand(this),
                RightClickCommand = new EdgeRightClickCommand(this, _graphModel),
            };

            Edges.Add(edge);
            GraphComponents.Add(edge);

            from.Edges.Add(edge);
            to.Edges.Add(edge);
        }

        private void Model_GraphLoaded(object? sender, GraphLocationEventArgs e)
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
                GraphComponents.Add(vertex);
            }
        }

        private void Model_AlgorithmInitialized(object? sender, EventArgs e)
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

        private void Model_AlgorithmCancelled(object? sender, EventArgs e)
        {
            ClearSelections();

            CanvasEnabled = true;
        }

        private void Model_NegativeCycleFound(object? sender, RouteEventArgs e)
        {
            SelectRoute(e.Route, true);
        }

        private void Model_RouteCreated(object? sender, RouteEventArgs e)
        {
            SelectRoute(e.Route, false);
        }

        private void Model_VertexRemoved(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(MaxVertexCountReached));
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
