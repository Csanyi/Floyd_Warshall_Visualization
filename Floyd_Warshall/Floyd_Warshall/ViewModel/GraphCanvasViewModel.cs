using Floyd_Warshall_Model;
using Floyd_Warshall.ViewModel.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using System.Windows.Automation;
using System.Windows.Input;

namespace Floyd_Warshall.ViewModel.GraphComponents
{
    public class GraphCanvasViewModel : ViewModelBase
    {
        private GraphModel _graphModel;

        private int _vertexId = 0;
        public int GetVertexId { get { return ++_vertexId; } }

        private int _edgeId = 0;
        public int GetEdgeId { get { return ++_edgeId; } }

        public VertexViewModel _selectedVertex = null;
        public VertexViewModel SelectedVertex { get { return _selectedVertex; } set { _selectedVertex = value; } }

        private EdgeViewModelBase _selectedEdge = null;
        public EdgeViewModelBase SelectedEdge
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

        private double _mouseX;
        public double MouseX
        {
            get { return _mouseX; }
            set 
            {    
                _mouseX = value; 
                OnPropertyChanged();
            }
        }

        private double _mouseY;
        public double MouseY
        {
            get { return _mouseY; }
            set
            {
                _mouseY = value;
                OnPropertyChanged();
            }
        }

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

            _graphModel.NewEmptyGraph += new EventHandler(Model_NewEmptyGraph);
            _graphModel.GraphLoaded += new EventHandler<IEnumerable<Tuple<Vertex, double, double>>>(Model_GraphLoaded);
        }

        public IEnumerable<Tuple<Vertex,double,double>> GetLocations() 
                        => Verteces.Select(v => new Tuple<Vertex, double, double>(v.Vertex,v.CanvasX, v.CanvasY));



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


        #region Model event handlers

        private void Model_NewEmptyGraph(object sender, EventArgs e) => Init();

        private void Model_GraphLoaded(object sender, IEnumerable<Tuple<Vertex, double, double>> e)
        {
            Init();

            foreach (var v in e)
            {
                if(v.Item1.Id >_vertexId) { _vertexId = v.Item1.Id; }

                VertexViewModel vertex = new VertexViewModel(v.Item1)
                {
                    CanvasX = v.Item2,
                    CanvasY = v.Item3,
                    IsSelected = false,
                    RightClickCommand = new VertexRightClickCommand(this, _graphModel),
                    LeftClickCommand = new VertexLeftClickCommand(this, _graphModel),
                };

                Verteces.Add(vertex);
                Views.Add(vertex);
            }

            foreach(Edge edge in _graphModel.Graph.GetEdges())
            {
                VertexViewModel from =  Verteces.Single(v => v.Vertex == edge.From);
                VertexViewModel to = Verteces.Single(v => (v.Vertex == edge.To));

                if (_graphModel.Graph.IsDirected)
                {               
                    DirectedEdgeViewModel edgevm = new DirectedEdgeViewModel(GetEdgeId, _graphModel.Graph)
                    {
                        From = from,
                        To = to,
                        Weight = edge.Weight,
                        IsSelected = false,
                        LeftClickCommand = new EdgeLeftClickCommand(this),
                        RightClickCommand = new EdgeRightClickCommand(this, _graphModel),
                    };

                    Edges.Add(edgevm);
                    from.Edges.Add(edgevm);
                    to.Edges.Add(edgevm);
                    Views.Add(edgevm);
                } else
                {
                    if (!from.Edges.Exists(e => (e.From == from && e.To == to) || (e.From == to && e.To == from)))
                    {
                        EdgeViewModel edgevm = new EdgeViewModel(GetEdgeId, _graphModel.Graph)
                        {
                            From = from,
                            To = to,
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
        }

        #endregion
    }
}
