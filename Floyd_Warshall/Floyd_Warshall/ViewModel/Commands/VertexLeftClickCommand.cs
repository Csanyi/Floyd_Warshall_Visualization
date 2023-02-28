using Floyd_Warshall_Model;
using Floyd_Warshall.ViewModel.GraphComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floyd_Warshall.ViewModel.Commands
{
    public class VertexLeftClickCommand : CommandBase
    {
        private GraphCanvasViewModel _vm;
        private GraphModel _graphModel;

        public VertexLeftClickCommand(GraphCanvasViewModel graphCanvasViewModel, GraphModel graphModel)
        {
            _vm = graphCanvasViewModel;
            _graphModel = graphModel;
        }

        public override void Execute(object parameter)
        {
            int id = Convert.ToInt32(parameter);

            VertexViewModel v = _vm.Verteces.Single(v => v.Id == id);

            if (_vm.SelectedVertex == null)
            {
                v.IsSelected = true;
                _vm.SelectedVertex = v;
            } else if (_vm.SelectedVertex == v)
            {
                v.IsSelected = false;
                _vm.SelectedVertex = null;
            } else
            {
                if (_graphModel.Graph.IsDirected)
                {
                    AddDirectedEdge(_vm.SelectedVertex, v);
                } else
                {
                    AddEdge(_vm.SelectedVertex, v);
                }

                _vm.SelectedVertex.IsSelected = false;
                _vm.SelectedVertex = null;
            }
        }

        private void AddEdge(VertexViewModel from, VertexViewModel to)
        {
            Vertex f = from.Vertex;
            Vertex t = to.Vertex;

            if (_graphModel.Graph.GetEdge(f, t) == null)
            {
                _graphModel.Graph.AddEdge(f, t, 1);

                EdgeViewModel edge = new EdgeViewModel(_vm.GetEdgeId, _graphModel.Graph)
                {
                    From = from,
                    To = to,
                    Weight = 1,
                    IsSelected = false,
                    LeftClickCommand = new EdgeLeftClickCommand(_vm),
                    RightClickCommand = new EdgeRightClickCommand(_vm, _graphModel),
                };

                _vm.Edges.Add(edge);
                _vm.Views.Add(edge);
                from.Edges.Add(edge);
                to.Edges.Add(edge);
            } else
            {
                from.Edges.Single(e => e.From == from && e.To == to || e.To == from && e.From == to).Weight++;
            }
        }

        private void AddDirectedEdge(VertexViewModel from, VertexViewModel to)
        {
            Vertex f = from.Vertex;
            Vertex t = to.Vertex;

            if (_graphModel.Graph.GetEdge(f, t) == null)
            {
                _graphModel.Graph.AddEdge(f, t, 1);

                DirectedEdgeViewModel edge = new DirectedEdgeViewModel(_vm.GetEdgeId, _graphModel.Graph)
                {
                    From = from,
                    To = to,
                    Weight = 1,
                    IsSelected = false,
                    LeftClickCommand = new EdgeLeftClickCommand(_vm),
                    RightClickCommand = new EdgeRightClickCommand(_vm, _graphModel),
                };

                _vm.Edges.Add(edge);
                _vm.Views.Add(edge);
                from.Edges.Add(edge);
                to.Edges.Add(edge);
            } else
            {
                from.Edges.Single(e => e.From == from && e.To == to).Weight++;
            }
        }
    }
}
